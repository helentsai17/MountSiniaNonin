﻿#pragma checksum "C:\Users\WRK_PC\source\repos\Mount Sinai Nonin device\Mount Sinai Nonin device\DataDisplay.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "14EDC8F115DCBECF7D74FC4CA99A065D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mount_Sinai_Nonin_device
{
    partial class DataDisplay : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private class DataDisplay_obj1_Bindings :
            global::Windows.UI.Xaml.Markup.IDataTemplateComponent,
            global::Windows.UI.Xaml.Markup.IXamlBindScopeDiagnostics,
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IDataDisplay_Bindings
        {
            private global::Mount_Sinai_Nonin_device.DataDisplay dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Windows.UI.Xaml.Controls.Button obj2;
            private global::Windows.UI.Xaml.Controls.Button obj3;
            private global::Windows.UI.Xaml.Controls.ComboBox obj12;
            private global::Windows.UI.Xaml.Controls.ComboBox obj13;
            private global::Windows.UI.Xaml.Controls.Button obj15;

            // Fields for each event bindings event handler.
            private global::Windows.UI.Xaml.RoutedEventHandler obj2Click;
            private global::Windows.UI.Xaml.RoutedEventHandler obj3Click;
            private global::Windows.UI.Xaml.Controls.SelectionChangedEventHandler obj12SelectionChanged;
            private global::Windows.UI.Xaml.Controls.SelectionChangedEventHandler obj13SelectionChanged;
            private global::Windows.UI.Xaml.RoutedEventHandler obj15Click;

            // Static fields for each binding's enabled/disabled state

            public DataDisplay_obj1_Bindings()
            {
            }

            public void Disable(int lineNumber, int columnNumber)
            {
                if (lineNumber == 75 && columnNumber == 76)
                {
                    this.obj2.Click -= obj2Click;
                }
                else if (lineNumber == 77 && columnNumber == 95)
                {
                    this.obj3.Click -= obj3Click;
                }
                else if (lineNumber == 32 && columnNumber == 23)
                {
                    this.obj12.SelectionChanged -= obj12SelectionChanged;
                }
                else if (lineNumber == 34 && columnNumber == 19)
                {
                    this.obj13.SelectionChanged -= obj13SelectionChanged;
                }
                else if (lineNumber == 26 && columnNumber == 66)
                {
                    this.obj15.Click -= obj15Click;
                }
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 2: // DataDisplay.xaml line 75
                        this.obj2 = (global::Windows.UI.Xaml.Controls.Button)target;
                        this.obj2Click = (global::System.Object p0, global::Windows.UI.Xaml.RoutedEventArgs p1) =>
                        {
                            this.dataRoot.CharacteristicReadButton_Click();
                        };
                        ((global::Windows.UI.Xaml.Controls.Button)target).Click += obj2Click;
                        break;
                    case 3: // DataDisplay.xaml line 77
                        this.obj3 = (global::Windows.UI.Xaml.Controls.Button)target;
                        this.obj3Click = (global::System.Object p0, global::Windows.UI.Xaml.RoutedEventArgs p1) =>
                        {
                            this.dataRoot.ValueChangedSubscribeToggle_Click();
                        };
                        ((global::Windows.UI.Xaml.Controls.Button)target).Click += obj3Click;
                        break;
                    case 12: // DataDisplay.xaml line 31
                        this.obj12 = (global::Windows.UI.Xaml.Controls.ComboBox)target;
                        this.obj12SelectionChanged = (global::System.Object p0, global::Windows.UI.Xaml.Controls.SelectionChangedEventArgs p1) =>
                        {
                            this.dataRoot.ServiceList_SelectionChanged();
                        };
                        ((global::Windows.UI.Xaml.Controls.ComboBox)target).SelectionChanged += obj12SelectionChanged;
                        break;
                    case 13: // DataDisplay.xaml line 33
                        this.obj13 = (global::Windows.UI.Xaml.Controls.ComboBox)target;
                        this.obj13SelectionChanged = (global::System.Object p0, global::Windows.UI.Xaml.Controls.SelectionChangedEventArgs p1) =>
                        {
                            this.dataRoot.CharacteristicList_SelectionChanged();
                        };
                        ((global::Windows.UI.Xaml.Controls.ComboBox)target).SelectionChanged += obj13SelectionChanged;
                        break;
                    case 15: // DataDisplay.xaml line 26
                        this.obj15 = (global::Windows.UI.Xaml.Controls.Button)target;
                        this.obj15Click = (global::System.Object p0, global::Windows.UI.Xaml.RoutedEventArgs p1) =>
                        {
                            this.dataRoot.ConnectButton_Click();
                        };
                        ((global::Windows.UI.Xaml.Controls.Button)target).Click += obj15Click;
                        break;
                    default:
                        break;
                }
            }

            // IDataTemplateComponent

            public void ProcessBindings(global::System.Object item, int itemIndex, int phase, out int nextPhase)
            {
                nextPhase = -1;
            }

            public void Recycle()
            {
                return;
            }

            // IDataDisplay_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            public void DisconnectUnloadedObject(int connectionId)
            {
                throw new global::System.ArgumentException("No unloadable elements to disconnect.");
            }

            public bool SetDataRoot(global::System.Object newDataRoot)
            {
                if (newDataRoot != null)
                {
                    this.dataRoot = (global::Mount_Sinai_Nonin_device.DataDisplay)newDataRoot;
                    return true;
                }
                return false;
            }

            public void Loading(global::Windows.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::Mount_Sinai_Nonin_device.DataDisplay obj, int phase)
            {
                if (obj != null)
                {
                }
            }
        }
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // DataDisplay.xaml line 75
                {
                    this.CharacteristicReadButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 3: // DataDisplay.xaml line 77
                {
                    this.ValueChangedSubscribeToggle = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 4: // DataDisplay.xaml line 79
                {
                    this.CharacteristicLatestValue = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 5: // DataDisplay.xaml line 68
                {
                    this.PAIDisplay = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 6: // DataDisplay.xaml line 59
                {
                    this.spO2Display = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 7: // DataDisplay.xaml line 51
                {
                    this.PITValue = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 8: // DataDisplay.xaml line 45
                {
                    this.HeartReteDataDisply = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 9: // DataDisplay.xaml line 37
                {
                    this.BatteryLevelData = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 10: // DataDisplay.xaml line 38
                {
                    this.TimeNow = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 11: // DataDisplay.xaml line 39
                {
                    this.ShareButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.ShareButton).Click += this.ShareData;
                }
                break;
            case 12: // DataDisplay.xaml line 31
                {
                    this.ServiceList = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                }
                break;
            case 13: // DataDisplay.xaml line 33
                {
                    this.CharacteristicList = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                }
                break;
            case 14: // DataDisplay.xaml line 24
                {
                    this.SelectedDeviceRun = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 15: // DataDisplay.xaml line 26
                {
                    this.ConnectButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 16: // DataDisplay.xaml line 27
                {
                    this.progressring = (global::Windows.UI.Xaml.Controls.ProgressRing)(target);
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
            switch(connectionId)
            {
            case 1: // DataDisplay.xaml line 1
                {                    
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)target;
                    DataDisplay_obj1_Bindings bindings = new DataDisplay_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Loading += bindings.Loading;
                    global::Windows.UI.Xaml.Markup.XamlBindingHelper.SetDataTemplateComponent(element1, bindings);
                }
                break;
            }
            return returnValue;
        }
    }
}

