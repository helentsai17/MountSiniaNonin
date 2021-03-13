using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Mount_Sinai_Nonin_device
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;


        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            ApplicationView.PreferredLaunchViewSize = new Size(850, 600);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 320));
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBoundsChanged += MainPage_VisibleBoundsChanged;
        }

        private void MainPage_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            var Width = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds.Width;
            if(Width >= 720)
            {
                SView.DisplayMode = SplitViewDisplayMode.CompactInline;
                SView.IsPaneOpen = true;
            }else if( Width >= 360)
            {
                SView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                SView.IsPaneOpen = false;
            }
            else
            {
                SView.DisplayMode = SplitViewDisplayMode.Overlay;
                SView.IsPaneOpen = false;
            }
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (InnerFrame.BackStack.Any())
            {
                e.Handled = true;
                InnerFrame.GoBack();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = InnerFrame.BackStack.Any() ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            InnerFrame.Navigate(typeof(ScanAndPair));
            //navigation buttom from system
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = InnerFrame.BackStack.Any() ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;

        }

        private void Goto_FindDevice(object sender, RoutedEventArgs e)
        {
            InnerFrame.Navigate(typeof(ScanAndPair));
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = InnerFrame.BackStack.Any() ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void Goto_SeeData(object sender, RoutedEventArgs e)
        {
            InnerFrame.Navigate(typeof(DataDisplay));
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = InnerFrame.BackStack.Any() ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void Goto_UserInfo(object sender, RoutedEventArgs e)
        {
            InnerFrame.Navigate(typeof(UserInfo));
        }

        private void Open_Menu(object sender, RoutedEventArgs e)
        {
            SView.IsPaneOpen = !SView.IsPaneOpen;
        }


        // Toast show notification
        private void ToastNotification(object sender, RoutedEventArgs e)
        {
            ToastContent content = new ToastContent()
            {
                Launch = "app-defined-string",
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Nonin device 3150"
                            },
                            new AdaptiveText()
                            {
                                Text = "device is not connected"
                            }

                        },
                    }
                },
          
                Audio = new ToastAudio()
                {
                    Src = new Uri("ms-winsoundevent:Notification.Reminder")
                }
            };

            var notification = new ToastNotification(content.GetXml());
            notification.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);
            ToastNotificationManager.CreateToastNotifier().Show(notification);

        }
    }
}
