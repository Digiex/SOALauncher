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
            OutputThread = new Thread(new ThreadStart(RunOutputThread));
        }
        MainWindow.VersionListItem Version;
        public void RunGame(MainWindow.VersionListItem version)
        {
            if (!GameThread.IsAlive)
            {
                Version = version;
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
            if (OutputThread.IsAlive)
            {
                OutputThread.Abort();
            }
        }
        Thread GameThread;
        Thread OutputThread;
        Process process = new Process();
        bool killProcess = false;

        private void RunGameThread()
        {
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
            OutputThread.Start();
            process.StandardInput.WriteLine(Settings.Default.Resolution + 1);
            process.StandardInput.WriteLine(Settings.Default.Fullscreen ? 1 : 0);
            //if (Directory.Exists(@"C:\Users\Jesse\Downloads\SoA_0.1.2.1.2\Release\Data\Saves\" + SaveName))
            //{
            //    process.StandardInput.WriteLine(1);
            //}
            //else
            //{
            //    process.StandardInput.WriteLine(0);
            //    process.StandardInput.WriteLine(SaveName);
            //    process.StandardInput.WriteLine(0);
            //}
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
            OutputThread.Abort();
        }

        private void RunOutputThread()
        {
            while (!process.HasExited)
            {
                AddToConsole(((char)process.StandardOutput.Read()).ToString());
            }

        }

        public void AddToConsole(string text)
        {
            Dispatcher.BeginInvoke(
                new Action<ConsoleTab>((sender) =>
                {
                    ConsoleTextBox.Text += text;
                    ConsoleTextBox.ScrollToEnd();
                }),
                new object[] { this }
                );
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!process.HasExited)
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