using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace SOALauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + System.IO.Path.DirectorySeparatorChar + "SOALauncher" + System.IO.Path.DirectorySeparatorChar;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                foreach (var dir in Directory.EnumerateDirectories(MainWindow.AppData + "Versions"))
                {
                    if (File.Exists(dir + Path.DirectorySeparatorChar + "Release" + Path.DirectorySeparatorChar + "SOA.exe"))
                    {
                        VersionList.Items.Add(new VersionListItem()
                        {
                            Name = Path.GetFileName(Path.GetDirectoryName(dir + System.IO.Path.DirectorySeparatorChar))
                        });
                    }
                }
                versionTab.VersionListBox.ItemsSource = VersionList.Items;
            }
            catch { }
            try
            {
                var wc = new WebClient();
                wc.DownloadStringCompleted += wc_DownloadStringCompleted;
                wc.DownloadStringAsync(new Uri("http://minecraft.digiex.org/SOALauncher/SOA/versions.php"));
            }
            catch { }
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var lines = e.Result.Split('\n');
                foreach (var line in lines)
                {
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    var opt = line.Split('|');
                    if (opt.Length > 1)
                    {
                        var item = new VersionListItem()
                         {
                             Name = opt[0],
                             URL = opt[1]
                         };
                        if (VersionList.Items.Contains(item))
                        {
                            VersionList.Items.Remove(item);
                        }
                        VersionList.Items.Add(item);
                    }
                }
                VersionList.SelectedIndex = VersionList.Items.Count - 1; //Select the last one, probably the newest
                versionTab.VersionListBox.ItemsSource = VersionList.Items;
                versionTab.VersionListBox.SelectedIndex = versionTab.VersionListBox.Items.Count - 1; //Select the last one, probably the newest
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (VersionList.SelectedItem == null)
            {
                return;
            }
            var item = (VersionListItem)VersionList.SelectedItem;
            if (item.IsDownloaded)
            {
                ConsoleTabItem.Visibility = System.Windows.Visibility.Visible;
                tabControl.SelectedItem = ConsoleTabItem;
                Console.RunGame(item);
            }
            else
            {
                tabControl.SelectedItem = versionTabItem;
                versionTab.Download(item);
            }
        }

        private void window_Closed(object sender, EventArgs e)
        {
            Console.EndGame();
        }

        private void VersionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VersionList.SelectedItem == null)
            {
                return;
            }
            var item = (VersionListItem)VersionList.SelectedItem;
            if (!item.IsDownloaded)
            {
                PlayButton.Content = "Download";
            }
            else
            {
                PlayButton.Content = "Play";
            }
        }
        public class VersionListItem : IEquatable<VersionListItem>
        {
            public string Name { get; set; }
            public string URL { get; set; }
            public string DirPath
            {
                get
                {
                    return MainWindow.AppData + "Versions" + System.IO.Path.DirectorySeparatorChar + Name + System.IO.Path.DirectorySeparatorChar;
                }
            }
            public bool IsDownloaded
            {
                get
                {
                    return File.Exists(DirPath + "Release" + System.IO.Path.DirectorySeparatorChar + "SOA.exe");
                }
            }
            public override string ToString()
            {
                return Name;
            }
            public bool Equals(VersionListItem obj)
            {
                return Name.Trim().ToLower().Equals(obj.Name.Trim().ToLower());
            }
            public override bool Equals(object obj)
            {
                if (obj is VersionListItem)
                {
                    return Equals((VersionListItem)obj);
                }
                return base.Equals(obj);
            }
        }

    }
}
