using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace project_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Integral integral;
        BackgroundWorker backgroundWorker;
        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker = (BackgroundWorker) this.Resources["worker"];

        }

        private void buttonParams_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            if(window1.ShowDialog() != true) { return; }

            integral = window1.integral;
            MessageBox.Show(integral.ToString());
        }

        private void buttonD_Click(object sender, RoutedEventArgs e)
        {
            if(integral == null) { return; }

            Thread thread = new Thread(Calculate);
            thread.Start(); 
        }

        private void buttonW_Click(object sender, RoutedEventArgs e)
        {
            buttonD.IsEnabled = false; 
            buttonW.IsEnabled =false;
            backgroundWorker.RunWorkerAsync();
        }

        private async void buttonA_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();

            if(integral == null) { return; }

            var data = integral.GetDoublesAsync();

            await foreach( var item in data)
            {
                listBox.Items.Add($"x = {item.Item1:0.00} | S = {item.Item2:0.000000}");

                pBar.Value = item.Item3;
            }

        }

        private void Calculate()
        {
            if(integral == null) { return; }

            int n = integral.N;
            double h = (integral.B - integral.A) / n;
            double step = n / 100.0;
            double S = 0;
            for(int i = 0; i < n; i++)
            {
                double x = integral.A + h* i;
                S += integral.Func(x) * h;
                if(i%step == 0)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        new Action(() => pBar.Value = i / step
                        ));
                }
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
                    () => listBox.Items.Add($"S = {S:0.000000}")
                    ));
                Thread.Sleep(50);
            }
            MessageBox.Show($"S= {S:0.000000}");
        }

        private void BackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (integral == null) { return; }

            int n = integral.N;
            double h = (integral.B - integral.A) / n;
            double step = n / 100.0;
            double S = 0;
            for (int i = 0; i < n; i++)
            {
                double x = integral.A + h * i;
                S += integral.Func(x) * h;
                if (i % step == 0 && backgroundWorker != null && backgroundWorker.WorkerReportsProgress)
                {
                    backgroundWorker.ReportProgress((int)(i / step));
                }
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
                    () => listBox.Items.Add($"S = {S:0.000000}")
                    ));
                Thread.Sleep(50);
            }
            MessageBox.Show($"S= {S:0.000000}");
        }

        private void BackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            pBar.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            buttonD.IsEnabled = true;
            buttonW.IsEnabled = true;
        }
    }
}
