using SOALauncher.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            GameThread = new Thread(new ThreadStart(RunGameThread));
        }
        private string SaveName;
        public void RunGame(string savename)
        {
            if (!GameThread.IsAlive)
            {
                SaveName = savename;
                GameThread.Start();
            }
            else
            {
                MessageBox.Show("Game already running!");
            }
        }
        public void EndGame()
        {
            killProcess = true;
            if (GameThread.IsAlive)
            {
                GameThread.Abort();
            }
        }
        Thread GameThread;
        Process process = new Process();
        bool killProcess = false;

        private void RunGameThread()
        {
            var processStartInfo = new ProcessStartInfo(@"C:\Users\Jesse\Downloads\SoA_0.1.2.1.2\Release\SOA.exe", "");
            processStartInfo.WorkingDirectory = @"C:\Users\Jesse\Downloads\SoA_0.1.2.1.2\Release\";
            processStartInfo.UseShellExecute = false;
            processStartInfo.ErrorDialog = true;

            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.CreateNoWindow = true;


            process.StartInfo = processStartInfo;
            process.OutputDataReceived += process_OutputDataReceived;
            bool processStarted = process.Start();
            process.BeginOutputReadLine();
            process.StandardInput.WriteLine(Settings.Default.Resolution + 1);
            process.StandardInput.WriteLine(Settings.Default.Fullscreen ? 1 : 0);
            if (Directory.Exists(@"C:\Users\Jesse\Downloads\SoA_0.1.2.1.2\Release\Data\Saves\" + SaveName))
            {
                process.StandardInput.WriteLine(1);
            }
            else
            {
                process.StandardInput.WriteLine(0);
                process.StandardInput.WriteLine(SaveName);
                process.StandardInput.WriteLine(0);
            }
            try
            {
                while (!process.HasExited)
                {
                    if (killProcess)
                    {
                        process.Kill();
                    }
                }
            }
            catch
            {
                process.Kill();
            }
        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }
            string line = e.Data;
            AddToConsole(line.Trim() + Environment.NewLine);
        }

        public void AddToConsole(string text)
        {
            Dispatcher.BeginInvoke(
                new Action<ConsoleTab>((sender) =>
                {
                    ConsoleTextBox.Text += text;
                    ScrollView.ScrollToBottom();
                }),
                new object[] { this }
                );
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            EndGame();
        }

        private void ConsoleTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            ConsoleTextBox.CaretIndex = ConsoleTextBox.Text.Length - 1;
            if (e.Key == Key.Enter)
            {
                var ary = ConsoleTextBox.Text.Split(Environment.NewLine.ToCharArray());
                process.StandardInput.WriteLine(ary[ary.Length - 1]);
            }
            else if (e.Key == Key.Return)
            {
                if (ConsoleTextBox.Text.EndsWith(Environment.NewLine))
                {
                    e.Handled = true;
                }
            }
        }
    }
}