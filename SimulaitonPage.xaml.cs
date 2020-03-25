using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.System.Threading;
using System.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MonopolyAnalysis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SimulationPage : Page
    {
        private int _playerAmount = 2;
        private int _gameAmount = 1;
        private bool isMultiThreadedExecution = false;
        public SimulationPage()
        {
            this.InitializeComponent();
        }

 
        private void NavigateToAnalysis(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AnalysisPage));
        }

        private void NavigateToAnalysis(object sender, KeyRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AnalysisPage));
        }

        private void Navigate(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            Debug.Print(args.InvokedItem.ToString());
    
            switch (args.InvokedItem.ToString())
            {
                case "Analysis":
                    this.Frame.Navigate(typeof(AnalysisPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
                    break;
                case "About":
                    this.Frame.Navigate(typeof(AboutPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
                    break;
            }
        }

        private void PlayerAmountSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            int sliderValue = Convert.ToInt32(e.NewValue);
            _playerAmount = sliderValue;
            string labelText = String.Format("Amount of players: {0}", _playerAmount);

            if (this.playerAmountLabel != null)
            {
                this.playerAmountLabel.Text = labelText;
            }
        }

        private void GameAmountSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            decimal sliderValue = Convert.ToDecimal(e.NewValue);
            decimal roundedSliderValue = Math.Round(sliderValue / 100, 0) * 100;
            _gameAmount = Convert.ToInt32(roundedSliderValue);
            Slider slider = (Slider)sender;
            slider.Value = _gameAmount;
            string labelText = String.Format("Amount of games: {0}", _gameAmount);

            if (this.gameAmountLabel != null)
            {
                this.gameAmountLabel.Text = labelText;
            }
        }

        private void MultiThreadedExecutionCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            isMultiThreadedExecution = Convert.ToBoolean(this.multiThreadedExecution.IsChecked);
        }
        int i;


        private void startSimulation(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (isMultiThreadedExecution)
            {
                asyncSimulations();
            }
            else
            {
                syncSimulations();
            }
        }

        private void syncSimulations()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (i = 1; i <= _gameAmount; i++)
            {
                DataCollector dataCollector = new DataCollector(_playerAmount);
                dataCollector.SimulationComplete += saveMoves;
                dataCollector.Start();

            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            simulationTime.Text = elapsedTime;
        }
        int counter = 1;
        Stopwatch stopWatch;
        TimeSpan ts;
        double processorCountValue;
        private static Mutex _asyncSimulationCounterMutex = new Mutex();
        async private void done()
        {
            System.Diagnostics.Debug.WriteLine("Done" + counter);
            if (counter == processorCountValue)
            {
                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    simulationTime.Text = elapsedTime;
                });
            }
            _asyncSimulationCounterMutex.WaitOne();
            counter++;
            _asyncSimulationCounterMutex.ReleaseMutex();
        }
        private void asyncSimulations()
        {
            counter = 1;
            stopWatch = new Stopwatch();
            stopWatch.Start();
            processorCountValue = processorCount.Value;
            int split = _gameAmount / (int)processorCountValue;

            for (i = 1; i <= processorCountValue; i++)
            {
                _ = Windows.System.Threading.ThreadPool.RunAsync(
                (workItem) =>
                {
                    for (int f = 0; f < split; f++)
                    {
                        DataCollector dataCollector = new DataCollector(_playerAmount);
                        dataCollector.SimulationComplete += saveMoves;
                        dataCollector.Start();

                    }
                    done();
                }, WorkItemPriority.Low, WorkItemOptions.TimeSliced);
            }


        }

        private void saveMoves(object source, List<Move> moves, int playerAmount)
        {
        }

        private void ProcessorCount_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            double processorCountValue = processorCount.Value;
            if (Convert.ToBoolean(processorCountValue % 2))
            {
                processorCountValue++;
                processorCount.Value = processorCountValue;
            }
            processorCountLabel.Text = "Amount of virtual processors to split the tasks: " + processorCountValue;
        }
    }
}
