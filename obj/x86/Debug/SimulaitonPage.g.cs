﻿#pragma checksum "C:\Users\zitku\source\repos\MonolpolyAnalysis\SimulaitonPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "868371C848E7E58ACA653A61B3AA2271"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MonopolyAnalysis
{
    partial class SimulationPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // SimulaitonPage.xaml line 26
                {
                    this.title = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 3: // SimulaitonPage.xaml line 93
                {
                    this.simulationTime = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 4: // SimulaitonPage.xaml line 94
                {
                    this.simulationButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.simulationButton).Click += this.startSimulation;
                }
                break;
            case 5: // SimulaitonPage.xaml line 89
                {
                    this.processorCountLabel = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 6: // SimulaitonPage.xaml line 90
                {
                    this.processorCount = (global::Windows.UI.Xaml.Controls.Slider)(target);
                    ((global::Windows.UI.Xaml.Controls.Slider)this.processorCount).ValueChanged += this.ProcessorCount_ValueChanged;
                }
                break;
            case 7: // SimulaitonPage.xaml line 86
                {
                    this.multiThreadedExecution = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.multiThreadedExecution).Click += this.MultiThreadedExecutionCheckbox_Click;
                }
                break;
            case 8: // SimulaitonPage.xaml line 79
                {
                    this.gameAmountLabel = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 9: // SimulaitonPage.xaml line 80
                {
                    this.gameAmountSlider = (global::Windows.UI.Xaml.Controls.Slider)(target);
                    ((global::Windows.UI.Xaml.Controls.Slider)this.gameAmountSlider).ValueChanged += this.GameAmountSlider_ValueChanged;
                }
                break;
            case 10: // SimulaitonPage.xaml line 73
                {
                    this.playerAmountLabel = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 11: // SimulaitonPage.xaml line 74
                {
                    this.playerAmountSlider = (global::Windows.UI.Xaml.Controls.Slider)(target);
                    ((global::Windows.UI.Xaml.Controls.Slider)this.playerAmountSlider).ValueChanged += this.PlayerAmountSlider_ValueChanged;
                }
                break;
            case 12: // SimulaitonPage.xaml line 35
                {
                    this.textBlock33 = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 13: // SimulaitonPage.xaml line 36
                {
                    this.textBlocke33 = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 14: // SimulaitonPage.xaml line 23
                {
                    global::Windows.UI.Xaml.Controls.Button element14 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)element14).Click += this.NavigateToAnalysisPage;
                }
                break;
            case 15: // SimulaitonPage.xaml line 24
                {
                    global::Windows.UI.Xaml.Controls.Button element15 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)element15).Click += this.NavigateToAboutPage;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

