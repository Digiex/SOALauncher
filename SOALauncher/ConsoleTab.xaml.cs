using SOALauncher.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for ConsoleTab.xaml
    /// </summary>
    public partial class ConsoleTab : UserControl
    {
        public ConsoleTab()
        {
            this.InitializeComponent();
            process.Exited += process_Exited;
        }
        bool gameRunning = false;
        void process_Exited(object sender, EventArgs e)
        {
            gameRunning = false;
        }
        MainWindow.VersionListItem Version;
        public async void RunGame(MainWindow.VersionListItem version)
        {
            if (!gameRunning)
            {
                Version = version;
                try
                {
                    await StartGameAsync();
                    InputBox.Focus();
                    try
                    {
                        await RunOutputAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to read console output: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to start the game: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    gameRunning = false;
                }
            }
            else
            {
                MessageBox.Show("Game already running!");
            }
        }
        public void EndGame()
        {
            if (gameRunning && process.MainWindowTitle.Length == 0)
            {
                process.Kill();
            }
        }
        Process process = new Process();

        private Task StartGameAsync()
        {
            return Task.Run(() =>
            {
                gameRunning = true;
                var savedir = Version.DirPath + "Release" + System.IO.Path.DirectorySeparatorChar + "Data" + System.IO.Path.DirectorySeparatorChar + "Saves";
                if (!Directory.Exists(savedir))
                {
                    Directory.CreateDirectory(savedir);
                }
                var processStartInfo = new ProcessStartInfo(Version.DirPath + "Release" + System.IO.Path.DirectorySeparatorChar + "SOA.exe", "");
                processStartInfo.WorkingDirectory = Version.DirPath + "Release" + System.IO.Path.DirectorySeparatorChar;
                processStartInfo.UseShellExecute = false;
                processStartInfo.ErrorDialog = true;

                processStartInfo.RedirectStandardError = true;
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow = true;


                process.StartInfo = processStartInfo;
                bool processStarted = process.Start();
                process.StandardInput.WriteLine(Settings.Default.Resolution + 1);
                process.StandardInput.WriteLine(Settings.Default.Fullscreen ? 1 : 0);
            });
        }

        private Task RunOutputAsync()
        {
            return Task.Run(() =>
            {
                while (!process.HasExited)
                {
                    string text = ((char)process.StandardOutput.Read()).ToString();
                    AddToConsole(text);
                }
            });

        }

        public void AddToConsole(string text)
        {
            Dispatcher.InvokeAsync(() =>
                {
                    ConsoleTextBox.AppendText(text);
                    ConsoleTextBox.ScrollToEnd();
                });
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (gameRunning)
                {
                    AddToConsole(InputBox.Text + Environment.NewLine);
                    process.StandardInput.WriteLine(InputBox.Text);
                    InputBox.Text = "";
                }
                e.Handled = true;
            }
        }
    }
}