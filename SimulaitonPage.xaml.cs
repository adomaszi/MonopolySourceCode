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
using Monopoly;

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
        private bool _isMultiThreadedExecution = false;
        private List<Move> _moves = new List<Move>();
        private readonly object _movesLock = new object();
        private static Mutex _asyncSimulationCounterMutex = new Mutex();
        private int _asyncSimulationCounter = 1;
        private double _processorCountValue;
        
        public SimulationPage()
        {
            this.InitializeComponent();

            UpdateRecordCount();
        }

        private void NavigateToAnalysisPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AnalysisPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
        }

        private void NavigateToAboutPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
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
            _isMultiThreadedExecution = Convert.ToBoolean(this.multiThreadedExecution.IsChecked);
        }

        private void StartSimulation(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_isMultiThreadedExecution)
            {
                AsyncSimulations();
            }
            else
            {
                SyncSimulations();
            }
        }

        private void SyncSimulations()
        {
            UpdateProgress("Simulations started");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 1; i <= _gameAmount; i++)
            {
                DataCollector dataCollector = new DataCollector(_playerAmount);
                dataCollector.SimulationComplete += SaveMoves;
                dataCollector.Start();

            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            UpdateTimer(ts);
            Debug.Print(_moves.Count.ToString());
            UpdateProgress("Simulations finished");
        }
        
        private bool IsDone()
        {
            if (_asyncSimulationCounter != _processorCountValue)
            {
                _asyncSimulationCounterMutex.WaitOne();
                _asyncSimulationCounter++;
                _asyncSimulationCounterMutex.ReleaseMutex();
                string progressString = String.Format("{0} out of {1} processor cores done", _asyncSimulationCounter, _processorCountValue);
                UpdateProgress(progressString);
                return false;
            } else
            {
                return true;
            }
        }

        private void AsyncSimulations()
        {
            UpdateProgress("Simulations started");
            _asyncSimulationCounter = 1;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            _processorCountValue = processorCount.Value;
            int split = _gameAmount / (int)_processorCountValue;

            for (int i = 1; i <= _processorCountValue; i++)
            {
                 _ = Windows.System.Threading.ThreadPool.RunAsync(
                (workItem) =>
                {
                    for (int f = 0; f < split; f++)
                    {
                        DataCollector dataCollector = new DataCollector(_playerAmount);
                        dataCollector.SimulationComplete += SaveMoves;
                        dataCollector.Start();
                    }

                    if (IsDone())
                    {
                        stopWatch.Stop();
                        // Get the elapsed time as a TimeSpan value.
                        TimeSpan ts = stopWatch.Elapsed;
                        UpdateTimer(ts);
                        
                    }
                }, WorkItemPriority.Low, WorkItemOptions.TimeSliced);
            }
        }

        async private void UpdateTimer(TimeSpan ts)
        {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10);
 
            // Update the timer
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                simulationTime.Text = elapsedTime;
            });
        }

        async private void UpdateProgress(string text)
        {
            // Update the progress
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                simulationProgress.Text = text;
                UpdateRecordCount();
            });
        }

        private void UpdateRecordCount()
        {
            twoPlayersRecordAmount.Text = DataAccess.GetNumberOfStoredGames(2).ToString();
            threePlayersRecordAmount.Text = DataAccess.GetNumberOfStoredGames(3).ToString();
            fourPlayersRecordAmount.Text = DataAccess.GetNumberOfStoredGames(4).ToString();
            fivePlayersRecordAmount.Text = DataAccess.GetNumberOfStoredGames(5).ToString();
            sixPlayersRecordAmount.Text = DataAccess.GetNumberOfStoredGames(6).ToString();
            sevenPlayersRecordAmount.Text = DataAccess.GetNumberOfStoredGames(7).ToString();
            eightPlayersRecordAmount.Text = DataAccess.GetNumberOfStoredGames(8).ToString();
        }

        
        private void SaveMoves(object source, List<Move> moves, int playerAmount, Board board)
        {
            lock (_movesLock)
            {
                foreach (Move move in moves)
                {
                    this._moves.Add(move);
                }
                string progressString = String.Format("{0} out of {1} processor cores done", _asyncSimulationCounter, _gameAmount);
                UpdateProgress(progressString);
                Debug.WriteLine(this._moves.Count);
            }
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

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }
    }
}
