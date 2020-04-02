﻿using Monopoly;
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
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MonopolyAnalysis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnalysisPage : Page
    {
        string _databaseConnectionString;
        // Analyser analyser;
        IEnumerable<Move> moves;
        IEnumerable<Player> _players;

        private int _playerAmount = 2;
        private bool _isMultiThreadedExecution = false;

        public AnalysisPage()
        {
            this.InitializeComponent();
            Debug.Write("Analyse init ======================================");
            // Remove them from here and put them into the single/multi threaded analysis
            //AnalyzeWinnerAverageRoll();
            //AnalyzeLoserAverageRoll();
            //AnalyzeTotalBestProperties();
            //AnalyzeWinnerBestProperties();
            //AnalyzeLoserBestProperties();
        }


        private void StartAnalysisHandler(object sender, RoutedEventArgs e)
        {

            if (_isMultiThreadedExecution)
            {
                MultiThreadedAnalysis();
            }
            else
            {
                SingeThreadedAnalysis();
            }
        }

        private void MultiThreadedAnalysis()
        {
            simulationProgress.Text = "Analysis started";

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            AnalyzeWinnerAverageRoll();
            AnalyzeLoserAverageRoll();
            AnalyzeTotalBestProperties();
            AnalyzeWinnerBestProperties();
            AnalyzeLoserBestProperties();


            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            UpdateTimer(ts);
            simulationProgress.Text = "Analysis finished";
        }

        private void SingeThreadedAnalysis()
        {
            simulationProgress.Text = "Analysis started";

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            AnalyzeWinnerAverageRoll();
            AnalyzeLoserAverageRoll();
            AnalyzeTotalBestProperties();
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
                Debug.WriteLine(elapsedTime);
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
                //decimal de = (decimal) roll / sum * 100;
                //Debug.WriteLine($"I: {i}, percent: {de}");
            }

            Debug.WriteLine($"Winner on average roll: {diceRoll}");
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


                //int rollAmount = (from element in listt
                //            where element == i
                //            select element).Sum();

                if (heighest < rollAmount)
                {
                    heighest = rollAmount;
                    diceRoll = i;
                }
            }

            Debug.WriteLine($"Loser on average roll: {diceRoll}");
        }

        private void AnalyzeWinnerBestProperties()
        {
            Dictionary<String, int> dict = DataAccess.GetWinnerPropertyRevenue(_playerAmount);
            List<int> flattenList = dict.Values.ToList();
            //var firstFiveArrivals = flattenList.OrderByDescending(i => i).Take(5);
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

            Debug.WriteLine("Winner");
            foreach (int i in firstFiveArrivals)
            {
                var myKey = dict.FirstOrDefault(x => x.Value == i).Key;
                Debug.WriteLine($"I from dic : {i} and property : {myKey}");
            }
        }

        private void AnalyzeLoserBestProperties()
        {
            Dictionary<String, int> dict = DataAccess.GetLoserPropertyRevenue(_playerAmount); ;
            List<int> flattenList = dict.Values.ToList();
            var firstFiveArrivals = (from t in flattenList
                                     orderby t
                                     select t).Take(5);

            Debug.WriteLine("Loser");
            foreach (int i in firstFiveArrivals)
            {
                var myKey = dict.FirstOrDefault(x => x.Value == i).Key;
                Debug.WriteLine($"I from dic : {i} and property : {myKey}");
            }
        }

        private void AnalyzeTotalBestProperties()
        {
            Dictionary<String, int> dict = DataAccess.GetAllPropertyRevenue(_playerAmount);
            List<int> flattenList = dict.Values.ToList();
            var firstFiveArrivals = (from t in flattenList
                                     orderby t descending
                                     select t).Take(1);

            Debug.WriteLine("All");
            foreach (int i in firstFiveArrivals)
            {
                var myKey = dict.FirstOrDefault(x => x.Value == i).Key;
                Debug.WriteLine($"I from dic : {i} and property : {myKey}");
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

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SingeThreadedAnalysis();
        }
    }
}
