using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
        userinfomation[] _DefaultUsers = new userinfomation[]
        {
            new userinfomation()
            {
                firstName = "helen",
                lastName = "tsai",
                address ="329 west", 
                email = "tt35810n",
                phoneNumber = "9177041172",

            },
        };

        private async void create_local_userinfo_click(object sender, RoutedEventArgs e)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.TryGetItemAsync(userfilename) as IStorageFile;

            if(file == null)
            {
                _user = _DefaultUsers;
                var newfile = await folder.CreateFileAsync(userfilename, CreationCollisionOption.ReplaceExisting);
                var text = JsonConvert.SerializeObject(_user);
                await FileIO.WriteTextAsync(newfile, text);
            }
            else
            {
                var text = await FileIO.ReadTextAsync(file);
                _user = JsonConvert.DeserializeObject<userinfomation[]>(text);
            }

            ShowUser();
        }

        private void edit_local_userinfo(object sender, RoutedEventArgs e)
        {
            //ToDo
        }


        private void ShowUser()
        {
            var userinfotext = String.Join(",", _user.Select(s => $"First Name : {s.firstName}, Last Name : {s.lastName}, address: {s.address}, phone number:{s.phoneNumber}, email: {s.email}"));
            personInfo.Text = userinfotext;
        }

       
    }

    public class userinfomation
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }

    }
}
