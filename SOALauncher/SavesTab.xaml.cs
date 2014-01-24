using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;

namespace SOALauncher
{
    /// <summary>
    /// Interaction logic for SavesTab.xaml
    /// </summary>
    public partial class SavesTab : UserControl
    {
        public SavesTab()
        {
            this.InitializeComponent();
            if (!Directory.Exists(MainWindow.AppData + "Versions"))
            {
                Directory.CreateDirectory(MainWindow.AppData + "Versions");
            }
            var dir2path = System.IO.Path.DirectorySeparatorChar + "Release" + System.IO.Path.DirectorySeparatorChar + "Data" + System.IO.Path.DirectorySeparatorChar + "Saves";
            foreach (var dir in Directory.GetDirectories(MainWindow.AppData + "Versions"))
            {
                var itm = new TreeViewItem();
                itm.Header = new DirectoryInfo(dir).Name;

                if (!Directory.Exists(dir + dir2path))
                {
                    Directory.CreateDirectory(dir + dir2path);
                }
                foreach (var dir2 in Directory.GetDirectories(dir + dir2path))
                {
                    itm.Items.Add(new TreeViewItem()
                    {
                        Header = new DirectoryInfo(dir2).Name
                    });
                }
                SaveTree.Items.Add(itm);
            }
        }
    }
}