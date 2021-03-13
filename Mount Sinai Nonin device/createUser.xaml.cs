using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class createUser : Page
    {
        public createUser()
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

        private async void Create_UserClick(object sender, RoutedEventArgs e)
        {

            string userfilename = "userinfo.json";
            userinfomation[] _user = Array.Empty<userinfomation>();

            userinfomation[] CreateUsers = new userinfomation[]
          {
            new userinfomation()
            {
                firstName = this.firstName,
                lastName = this.lastName,
                address =this.address,
                email = this.email,
                phoneNumber = this.phone,

            },
          };

            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.TryGetItemAsync(userfilename) as IStorageFile;

            if (file == null)
            {
                _user = CreateUsers;
                var newfile = await folder.CreateFileAsync(userfilename, CreationCollisionOption.ReplaceExisting);
                var text = JsonConvert.SerializeObject(_user);
                await FileIO.WriteTextAsync(newfile, text);
            }
            
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(_MainViewId);
            Window.Current.Close();


        }

        private async void Cancel_CreateClick(object sender, RoutedEventArgs e)
        {
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(_MainViewId);
            Window.Current.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            firstName = firstNameInput.Text;
            lastName = lastNameInput.Text;
            address = addressInput.Text;
            email = emailInput.Text;
            phone = phoneInput.Text;
        }
    }
}
