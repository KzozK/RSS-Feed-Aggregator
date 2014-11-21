using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using System.Runtime.InteropServices.WindowsRuntime;

namespace FeedReader
{
    public partial class Login : PhoneApplicationPage
    {
        public const string fileName = "UserDetails";
        UserDetail user;

        public Login() // CALLED FIRST, BEFORE MAINPAGE
        {
            InitializeComponent();
            restore();
        }

        private async void restore()
        {
            user = await restoreUserDetailAsync();
        }

        public async Task<UserDetail> restoreUserDetailAsync()
        {
            UserDetail userDetail = new UserDetail();
            try
            {
                //no exception means file exists
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                IRandomAccessStream inStream = await file.OpenReadAsync();
                // Deserialize the Session State.
                DataContractSerializer serializer = new DataContractSerializer(typeof(UserDetail));
                userDetail = (UserDetail)serializer.ReadObject(inStream.AsStreamForRead());
                inStream.Dispose();
            }
            catch (FileNotFoundException ex)
            {
                //find out through exception 
                return userDetail;
            }
            return userDetail;
        }

        public async Task saveUserDetailAsync() // store data in local file
        {
            try
            {
                StorageFile userdetailsfile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                IRandomAccessStream raStream = await userdetailsfile.OpenAsync(FileAccessMode.ReadWrite);
                using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
                {
                    // Serialize the Session State.
                    DataContractSerializer serializer = new DataContractSerializer(typeof(UserDetail));
                    serializer.WriteObject(outStream.AsStreamForWrite(), user);
                    await outStream.FlushAsync();
                }
            }
            catch (FileNotFoundException ex)
            {
                //find out through exception 
            }
        }

        private bool checkLogInPassword()
        {
            if (loginBox.Text.Count() > 0 && passwordBox.Password.Count() > 0)
            {
                return true;
            }
            return false;
        }

        private async void Connection_Click(object sender, RoutedEventArgs e)
        {
            if (checkLogInPassword())
            {
                user.login = loginBox.Text;
                user.password = passwordBox.Password;
                await saveUserDetailAsync(); // sa plante apres 3 appel a verifier
                NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
            }
        }
        private void Create_Account_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Auth/CreateAccount.xaml", UriKind.Relative));
        }
    }
}