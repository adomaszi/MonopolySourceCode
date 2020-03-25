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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MonopolyAnalysis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
        }
        private void Navigate(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
 

            switch (args.InvokedItem.ToString())
            {
                case "Simulation":
                    this.Frame.Navigate(typeof(SimulationPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
                    break;
                case "Analysis":
                    this.Frame.Navigate(typeof(AnalysisPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
                    break;
            }
        }
    }
}
