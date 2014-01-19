using SOALauncher.Properties;
using System;
using System.Collections.Generic;
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

namespace SOALauncher
{
    /// <summary>
    /// Interaction logic for SettingsTab.xaml
    /// </summary>
    public partial class SettingsTab : UserControl
    {
        public SettingsTab()
        {
            this.InitializeComponent();
            ResolutionBox.Items.Add("640x360");
            ResolutionBox.Items.Add("800x600");
            ResolutionBox.Items.Add("854x480");
            ResolutionBox.Items.Add("1280x720");
            ResolutionBox.Items.Add("1360x768");
            ResolutionBox.Items.Add("1366x768");
            ResolutionBox.Items.Add("1600x900");
            ResolutionBox.Items.Add("1920x1080");
            ResolutionBox.Items.Add("2560x1440");
            ResolutionBox.SelectedIndex = Settings.Default.Resolution;
            FullscreenBox.IsChecked = Settings.Default.Fullscreen;
        }

        private void ResolutionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.Default.Resolution = ResolutionBox.SelectedIndex;
            Settings.Default.Save();
        }

        private void FullscreenBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.Fullscreen = (bool)FullscreenBox.IsChecked;
            Settings.Default.Save();
        }
    }
}