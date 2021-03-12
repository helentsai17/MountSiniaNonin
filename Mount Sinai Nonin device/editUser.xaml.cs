using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mount_Sinai_Nonin_device
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class editUser : Page
    {
        public editUser()
        {
            this.InitializeComponent();
        }

        string firstName = "";
        string lastName = "";
        string address = "";
        string email = "";
        string phone = "";

        int _MainViewId;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _MainViewId = (int)e.Parameter;
        }

        private void Edit_UserClick(object sender, RoutedEventArgs e)
        {
           
        }

        private async void Cancel_EditClick(object sender, RoutedEventArgs e)
        {
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(_MainViewId);
            Window.Current.Close();
        }
    }
}
