using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            ExtendedVisualStateManager.GoToElementState(this as FrameworkElement, "Startup", false);
            ExtendedVisualStateManager.GoToElementState(this as FrameworkElement, "ShowControls", true);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this.window,"Startup",false);
            //Startup.Storyboard.Begin();
            //ExtendedVisualStateManager.GoToElementState(this as FrameworkElement, "ShowControls", true);
        }
    }
}
