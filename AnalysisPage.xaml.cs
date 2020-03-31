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

        public AnalysisPage()
        {
            this.InitializeComponent();
        }

        private void StartAnalysisHandler()
        {

        }

        private void MultiThreadedAnalysis()
        {

        }

        private void SingeThreadedAnalysis()
        {

        }

        private void AnalyzeWinnerAverageRoll()
        {

        }

        private void AnalyzeLoserAverageRoll()
        {

        }

        private void AnalyzeWinnerBestProperties()
        {

        }

        private void AnalyzeLoserBestProperties()
        {

        }

        private void AnalyzeTotalBestProperties()
        {

        }

        private void NavigateToSimulaitonPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SimulationPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
        }

        private void NavigateToAboutPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
        }


    }
}
