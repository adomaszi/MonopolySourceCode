using Monopoly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.ApplicationModel.Appointments;
using System.Threading.Tasks;
using System.Threading;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MonopolyAnalysis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnalysisPage : Page
    {
        private int _playerAmount = 2;
        private bool _isMultiThreadedExecution = false;

        private static Mutex _asyncAnalysisCounterMutex = new Mutex();
        private int _asyncAnalysisCounter = 1;

        public AnalysisPage()
        {
            this.InitializeComponent();
        }


        private void StartAnalysisHandler(object sender, RoutedEventArgs e)
        {
            UpdateProgressRing(true);
            if (_isMultiThreadedExecution)
            {
                MultiThreadedAnalysis();
            }
            else
            {
                SingeThreadedAnalysis();
            }
        }

        private async void MultiThreadedAnalysis()
        {
            simulationProgress.Text = "Analysis started";

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            await Task.Run(() => RunAllAnalyses());

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            UpdateTimer(ts);
            simulationProgress.Text = "Analysis finished";
        }

        private void RunAllAnalyses()
        {
            Task winnerAverage = Task.Run(() => AnalyzeWinnerAverageRoll());
            Task loserAverage = Task.Run(() => AnalyzeLoserAverageRoll());
            Task mostLandedOn = Task.Run(() => AnalyzeMostLandedOnProperty());
            Task winnerBestProperties = Task.Run(() => AnalyzeWinnerBestProperties());
            Task loserBestProperties = Task.Run(() => AnalyzeLoserBestProperties());

            winnerAverage.Wait();
            loserAverage.Wait();
            mostLandedOn.Wait();
            winnerBestProperties.Wait();
            loserBestProperties.Wait();
        }

        private void SingeThreadedAnalysis()
        {
            simulationProgress.Text = "Analysis started";

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            AnalyzeWinnerAverageRoll();
            AnalyzeLoserAverageRoll();
            AnalyzeMostLandedOnProperty();
            AnalyzeWinnerBestProperties();
            AnalyzeLoserBestProperties();


            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            UpdateTimer(ts);
            simulationProgress.Text = "Analysis finished";

        }

        async private void UpdateTimer(TimeSpan ts)
        {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                analysisTime.Text = elapsedTime;
            });
        }

        private void AnalyzeWinnerAverageRoll()
        {
            List<int> listt = DataAccess.GetRollsOfWinners(_playerAmount);
            int heighest = -1;
            int diceRoll = -1;
            for (int i = 1; i <= 12; i++)
            {
                int rollAmount = -1;
                if (_isMultiThreadedExecution)
                {
                    rollAmount = (from element in listt.AsParallel().WithDegreeOfParallelism(2)
                                      where element == i
                                      select element).Sum();
                } else
                {
                    rollAmount = (from element in listt.AsParallel()
                                      where element == i
                                      select element).Sum();
                }


                if (heighest < rollAmount)
                {
                    heighest = rollAmount;
                    diceRoll = i;
                }
            }

            updateAverageWinnerUI(diceRoll);
        }

        private async Task AsyncAnalyzeWinnerAverageRoll()
        {
            List<int> listt = DataAccess.GetRollsOfWinners(_playerAmount);
            int heighest = -1;
            int diceRoll = -1;
            for (int i = 1; i <= 12; i++)
            {
                int rollAmount = -1;
                rollAmount = (from element in listt.AsParallel().WithDegreeOfParallelism(2)
                                  where element == i
                                  select element).Sum();


                if (heighest < rollAmount)
                {
                    heighest = rollAmount;
                    diceRoll = i;
                }
            }

            updateAverageWinnerUI(diceRoll);
        }

        private void AnalyzeLoserAverageRoll()
        {
            List<int> listt = DataAccess.GetRollsOfLosers(_playerAmount);
            int heighest = -1;
            int diceRoll = -1;
            for (int i = 1; i <= 12; i++)
            {
                int rollAmount = -1;
                if (_isMultiThreadedExecution)
                {
                    rollAmount = (from element in listt.AsParallel().WithDegreeOfParallelism(2)
                                  where element == i
                                  select element).Sum();
                }
                else
                {
                    rollAmount = (from element in listt.AsParallel()
                                  where element == i
                                  select element).Sum();
                }
                

                if (heighest < rollAmount)
                {
                    heighest = rollAmount;
                    diceRoll = i;
                }
            }
            
            updateAverageLoserUI(diceRoll);
        }

        private void AnalyzeWinnerBestProperties()
        {
            Dictionary<String, int> dict = DataAccess.GetWinnerPropertyRevenue(_playerAmount);
            List<int> flattenList = dict.Values.ToList();
            IEnumerable<int> firstFiveArrivals;
            if (_isMultiThreadedExecution)
            {
                firstFiveArrivals = (from t in flattenList.AsParallel().WithDegreeOfParallelism(2)
                                     orderby t descending
                                     select t).Take(5);
            }
            else
            {
                firstFiveArrivals = (from t in flattenList
                                     orderby t descending
                                     select t).Take(5);
            }
            updateBestWinnerProperties(firstFiveArrivals, flattenList.Count, dict);
        }

        private void AnalyzeLoserBestProperties()
        {
            Dictionary<String, int> dict = DataAccess.GetLoserPropertyRevenue(_playerAmount); ;
            List<int> flattenList = dict.Values.ToList();
            IEnumerable<int> firstFiveArrivals;
            if (_isMultiThreadedExecution)
            {
                firstFiveArrivals = (from t in flattenList.AsParallel().WithDegreeOfParallelism(2)
                                         orderby t descending
                                         select t).Take(5);
            }
            else
            {
                firstFiveArrivals = (from t in flattenList
                                         orderby t descending
                                         select t).Take(5);
            }

            updateBestLoserProperties(firstFiveArrivals, flattenList.Count, dict);
        }

        private void AnalyzeMostLandedOnProperty()
        {
            Dictionary<String, int> dict = DataAccess.GetAmountLandedOn(_playerAmount);
            List<int> flattenList = dict.Values.ToList();
            IEnumerable<int> mostLandedOnProperty;
            if (_isMultiThreadedExecution)
            {
                mostLandedOnProperty = (from t in flattenList.AsParallel().WithDegreeOfParallelism(2)
                                    orderby t descending
                                    select t).Take(1);
            }
            else
            {
                mostLandedOnProperty = (from t in flattenList
                                    orderby t descending
                                    select t).Take(1);
            }

            foreach (int i in mostLandedOnProperty)
            {
                var mostLandedOn = dict.FirstOrDefault(x => x.Value == i).Key;
                int average = i / flattenList.Count;
                updateMostLandedOnField(mostLandedOn, average);
            }
        }

        private void NavigateToSimulaitonPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SimulationPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
        }

        private void NavigateToAboutPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
        }

        private void MultiThreadedExecutionCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _isMultiThreadedExecution = Convert.ToBoolean(this.multiThreadedExecution.IsChecked);
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
        
        private async void updateAverageWinnerUI(int diceRoll)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                AverageRollWinner.Text = diceRoll.ToString();
                IsDone();
            });
        }

        private async void updateAverageLoserUI(int diceRoll)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                AverageRollLoser.Text = diceRoll.ToString();
                IsDone();
            });
        }

        private async void updateMostLandedOnField(String mostLandedOn, int landedOnAmount)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                int gameAmount = DataAccess.GetNumberOfStoredGames(_playerAmount);
                
                MostLandedOnProperty.Text = String.Format("{0} road was the field that was most landed on in general. In {1} games players landed on it {2} times", mostLandedOn, gameAmount, landedOnAmount);
                IsDone();
            });
        }

        private async void updateBestWinnerProperties(IEnumerable<int> firstFiveArrivals, int total, Dictionary<String, int> dict)
        {
            int counter = 1;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                foreach (int i in firstFiveArrivals)
                {
                    var propertyName = dict.FirstOrDefault(x => x.Value == i).Key;
                    int average = i / total;
                    switch(counter)
                    {
                        case 1:
                            FirstPropertyNameWinner.Text = propertyName;
                            FirstPropertyRevenueWinner.Text = average.ToString();
                            break;
                        case 2:
                            SecondPropertyNameWinner.Text = propertyName;
                            SecondPropertyRevenueWinner.Text = average.ToString();
                            break;
                        case 3:
                            ThirdPropertyNameWinner.Text = propertyName;
                            ThirdPropertyRevenueWinner.Text = average.ToString();
                            break;
                        case 4:
                            FourthPropertyNameWinner.Text = propertyName;
                            FourthPropertyRevenueWinner.Text = average.ToString();
                            break;
                        case 5:
                            FifthPropertyNameWinner.Text = propertyName;
                            FifthPropertyRevenueWinner.Text = average.ToString();
                            break;
                    }
                    counter++;
                }
                IsDone();
            });
        }

        private async void updateBestLoserProperties(IEnumerable<int> firstFiveArrivals, int total, Dictionary<String, int> dict)
        {
            int counter = 1;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                foreach (int i in firstFiveArrivals)
                {
                    var propertyName = dict.FirstOrDefault(x => x.Value == i).Key;
                    int average = i / total;
                    switch (counter)
                    {
                        case 1:
                            FirstPropertyNameLoser.Text = propertyName;
                            FirstPropertyRevenueLoser.Text = average.ToString();
                            break;
                        case 2:
                            SecondPropertyNameLoser.Text = propertyName;
                            SecondPropertyRevenueLoser.Text = average.ToString();
                            break;
                        case 3:
                            ThirdPropertyNameLoser.Text = propertyName;
                            ThirdPropertyRevenueLoser.Text = average.ToString();
                            break;
                        case 4:
                            FourthPropertyNameLoser.Text = propertyName;
                            FourthPropertyRevenueLoser.Text = average.ToString();
                            break;
                        case 5:
                            FifthPropertyNameLoser.Text = propertyName;
                            FifthPropertyRevenueLoser.Text = average.ToString();
                            break;
                    }
                    counter++;
                }
                IsDone();
            });
        }

        private void IsDone()
        {
            if (_asyncAnalysisCounter != 5)
            {
                _asyncAnalysisCounterMutex.WaitOne();
                _asyncAnalysisCounter++;
                _asyncAnalysisCounterMutex.ReleaseMutex();
            }
            else
            {
                UpdateProgressRing(false);
            }
        }

        async private void UpdateProgressRing(bool isActive)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                splashProgressRing.IsActive = isActive;
            });
        }
    }
}
