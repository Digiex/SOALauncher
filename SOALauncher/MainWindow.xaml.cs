using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            var wc = new WebClient();
            wc.DownloadStringCompleted += wc_DownloadStringCompleted;
            wc.DownloadStringAsync(new Uri("http://minecraft.digiex.org/SOALauncher/SOA/versions.php"));
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
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
                    VersionList.Items.Add(new VersionListItem()
                    {
                        Name = opt[0],
                        URL = opt[1]
                    });
                }
            }
            VersionList.SelectedIndex = VersionList.Items.Count - 1; //Select the last one, probably the newest
            versionTab.VersionListBox.ItemsSource = VersionList.Items;
            versionTab.VersionListBox.SelectedIndex = versionTab.VersionListBox.Items.Count - 1; //Select the last one, probably the newest
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
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
        public class VersionListItem
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
        }

    }
}
