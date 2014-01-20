using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            extractThread = new Thread(new ThreadStart(Extract));
            wc.DownloadProgressChanged += wc_DownloadProgressChanged;
            wc.DownloadFileCompleted += wc_DownloadFileCompleted;
        }

        WebClient wc = new WebClient();
        static string currentDownload = string.Empty;
        Thread extractThread;
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
                    Directory.Delete(ver.DirPath, true);
                }
            }
            else if (!wc.IsBusy && currentDownload == string.Empty)
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
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressBar.Value = e.ProgressPercentage;
        }

        void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            extractThread.Start();
            DownloadProgressBar.IsIndeterminate = true;
        }

        private void Extract()
        {
            FastZip fz = new FastZip();
            fz.ExtractZip(currentDownload, System.IO.Path.GetDirectoryName(currentDownload), null);
            File.Delete(currentDownload);
            Dispatcher.BeginInvoke(
                new Action<VersionTab>((sender) =>
                {
                    currentDownload = string.Empty;
                    DownloadProgressBar.IsIndeterminate = false;
                    DownloadProgressBar.Value = 0;
                    DownloadDeleteButton.IsEnabled = false;
                }),
                new object[] { this }
                );
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