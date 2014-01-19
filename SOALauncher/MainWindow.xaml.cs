using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public MainWindow()
        {
            InitializeComponent();
            PlayButton.IsEnabled = false;
            foreach (var save in Directory.GetDirectories(@"C:\Users\Jesse\Downloads\SoA_0.1.2.1.2\Release\Data\Saves"))
            {
                SaveList.Items.Add(save.Split('\\').Last());
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ConsoleTabItem.Visibility = System.Windows.Visibility.Visible;
            tabControl.SelectedItem = ConsoleTabItem;
            Console.RunGame(SaveList.Text);
        }

        private void window_Closed(object sender, EventArgs e)
        {
            Console.EndGame();
        }

        private void SaveList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlayButton.IsEnabled = (((string)SaveList.Text).Trim() != string.Empty);
        }

        private void SaveList_TextInput(object sender, TextCompositionEventArgs e)
        {
            PlayButton.IsEnabled = (((string)SaveList.Text).Trim() != string.Empty);
        }

        private void SaveList_KeyUp(object sender, KeyEventArgs e)
        {
            PlayButton.IsEnabled = (((string)SaveList.Text).Trim() != string.Empty);
        }


    }
}
