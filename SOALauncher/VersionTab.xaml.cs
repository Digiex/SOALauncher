using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SOALauncher
{
    /// <summary>
    /// Interaction logic for VersionTab.xaml
    /// </summary>
    public partial class VersionTab : UserControl
    {
        public VersionTab()
        {
            this.InitializeComponent();
            wc.DownloadProgressChanged += wc_DownloadProgressChanged;
            wc.DownloadFileCompleted += wc_DownloadFileCompleted;
        }

        WebClient wc = new WebClient();
        static string currentDownload = string.Empty;
        private void DownloadDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (VersionListBox.SelectedItem == null)
            {
                return;
            }
            var ver = (MainWindow.VersionListItem)VersionListBox.SelectedItem;
            if (ver.IsDownloaded)
            {
                if (MessageBox.Show("Are you sure you want to delete the version? All saved games will be lost!", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Directory.Delete(ver.DirPath, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to fully delete the folder! Please make sure the game is not running. Error: " + ex.Message, "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    VersionListBox_SelectionChanged(null, null);
                }
            }
            else if (!wc.IsBusy && currentDownload == string.Empty)
            {
                try
                {
                    if (Directory.Exists(ver.DirPath))
                    {
                        Directory.Delete(ver.DirPath, true);
                    }
                    Directory.CreateDirectory(ver.DirPath);
                    currentDownload = ver.DirPath + ver.Name + ".zip";
                    DownloadDeleteButton.IsEnabled = false;
                    wc.DownloadFileAsync(new Uri(ver.URL), currentDownload);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to begin download! " + ex.Message, "Download Start Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    currentDownload = "";
                    DownloadDeleteButton.IsEnabled = true;
                }
            }
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressBar.Value = e.ProgressPercentage;
        }

        async void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    await Extract();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Extraction failed! " + ex.Message, "Extraction Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Download failed! " + e.Error.Message, "Download Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            currentDownload = string.Empty;
            DownloadProgressBar.Value = 0;
            DownloadDeleteButton.IsEnabled = true;
            VersionListBox_SelectionChanged(null, null);
        }

        private Task Extract()
        {
            return Task.Run(() =>
            {
                using (ZipArchive archive = ZipFile.OpenRead(currentDownload))
                {
                    var count = archive.Entries.Count;
                    var i = 0;
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        i++;

                        Dispatcher.InvokeAsync(() =>
                           {
                               DownloadProgressBar.Value = (int)(((double)i / (double)count) * 100);
                           });
                        string path = Path.Combine(Path.GetDirectoryName(currentDownload), entry.FullName.Replace('/', Path.DirectorySeparatorChar));
                        if (entry.Name == "" || entry.FullName.EndsWith("/"))
                        {
                            Directory.CreateDirectory(path);
                        }
                        else
                        {
                            entry.ExtractToFile(path);
                        }
                    }
                }
                File.Delete(currentDownload);
            });
        }

        public void Download(MainWindow.VersionListItem version)
        {
            VersionListBox.SelectedItem = version;
            if (DownloadDeleteButton.IsEnabled)
                DownloadDeleteButton_Click(null, null);
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (VersionListBox.SelectedItem == null)
            {
                return;
            }
            var ver = (MainWindow.VersionListItem)VersionListBox.SelectedItem;
            Process.Start(ver.DirPath);
        }

        private void VersionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VersionListBox.SelectedItem == null)
            {
                DownloadDeleteButton.IsEnabled = false;
                OpenFolderButton.IsEnabled = false;
            }
            else
            {
                var ver = (MainWindow.VersionListItem)VersionListBox.SelectedItem;
                DownloadDeleteButton.IsEnabled = true;
                if (ver.IsDownloaded)
                {
                    DownloadDeleteButton.Content = "Delete";
                    OpenFolderButton.IsEnabled = true;
                }
                else
                {
                    DownloadDeleteButton.Content = "Download";
                    OpenFolderButton.IsEnabled = false;
                }
                if (currentDownload != string.Empty)
                {
                    DownloadDeleteButton.IsEnabled = false;
                }
            }
        }
    }
}