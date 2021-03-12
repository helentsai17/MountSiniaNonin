using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class UserInfo : Page
    {
        public UserInfo()
        {
            this.InitializeComponent();
        }

        string userfilename = "userinfo.json";
        userinfomation[] _user = Array.Empty<userinfomation>();

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
           
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.TryGetItemAsync(userfilename) as IStorageFile;

            if (file != null)
            {
                var text = await FileIO.ReadTextAsync(file);
                _user = JsonConvert.DeserializeObject<userinfomation[]>(text);
                ShowUser();
            }
            
        }

       
        //userinfomation[] _DefaultUsers = new userinfomation[]
        //{
        //    new userinfomation()
        //    {
        //        firstName = "helen",
        //        lastName = "tsai",
        //        address ="329 west",
        //        email = "tt35810n",
        //        phoneNumber = "9177041172",

        //    },
        //};

        //private async void create_local_userinfo_click(object sender, RoutedEventArgs e)
        //{
        //    var folder = ApplicationData.Current.LocalFolder;
        //    var file = await folder.TryGetItemAsync(userfilename) as IStorageFile;

        //    if(file == null)
        //    {
        //        _user = _DefaultUsers;
        //        var newfile = await folder.CreateFileAsync(userfilename, CreationCollisionOption.ReplaceExisting);
        //        var text = JsonConvert.SerializeObject(_user);
        //        await FileIO.WriteTextAsync(newfile, text);
        //    }
        //    else
        //    {
        //        var text = await FileIO.ReadTextAsync(file);
        //        _user = JsonConvert.DeserializeObject<userinfomation[]>(text);
        //    }

        //    ShowUser();
        //}

       


        private void ShowUser()
        {
            //var userinfotext = String.Join(",", _user.Select(s => $"First Name : {s.firstName}, Last Name : {s.lastName}, address: {s.address}, phone number:{s.phoneNumber}, email: {s.email}"));
            var getuserinfo = _user[0];
            //personInfo.Text = userinfotext;
            fNameOutput.Text = getuserinfo.firstName;
            LNameOutput.Text = getuserinfo.lastName;
            addressOutput.Text = getuserinfo.address;
            phoneOutput.Text = getuserinfo.phoneNumber;
            emailOutput.Text = getuserinfo.email;
        }


        /// <summary>
        /// create new windows to create new users
        /// </summary>
        int userMainViewID = 0;
        private async void Open_window_To_Create(object sender, RoutedEventArgs e)
        {
            userMainViewID = ApplicationView.GetForCurrentView().Id;
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewID = 0;
            await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
             {
                 Frame frame = new Frame();
                 frame.Navigate(typeof(createUser), userMainViewID);
                 Window.Current.Content = frame;
                 Window.Current.Activate();
                 newViewID = ApplicationView.GetForCurrentView().Id;
             });
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewID);
        }

        private async void edit_local_userinfo(object sender, RoutedEventArgs e)
        {
            userMainViewID = ApplicationView.GetForCurrentView().Id;
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewID = 0;
            await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(editUser), userMainViewID);
                Window.Current.Content = frame;
                Window.Current.Activate();
                newViewID = ApplicationView.GetForCurrentView().Id;
            });
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewID);
        }
    }

    
}
