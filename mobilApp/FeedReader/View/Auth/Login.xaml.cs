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
using System.Text;
using Newtonsoft.Json.Linq;

namespace FeedReader
{
    public partial class Login : PhoneApplicationPage
    {
        public const string fileName = "UserDetails";
        UserDetail user = new UserDetail();

        public Login()
        {
            InitializeComponent();
            //restore();
        }

        #region Save to local file

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

        #endregion

        private bool checkLogInPassword()
        {
            if (loginBox.Text.Count() > 0 && passwordBox.Password.Count() > 0)
            {
                return true;
            }
            return false;
        }

        private void Connection_Click(object sender, RoutedEventArgs e)
        {
            //    if (checkLogInPassword())
            //    {
            user.login = loginBox.Text;
            user.password = passwordBox.Password;

            HttpWebRequest requete = (HttpWebRequest)HttpWebRequest.Create("http://rssfeedagregator.azurewebsites.net/api/account/login");
            requete.Method = "POST";
            requete.ContentType = "application/x-www-form-urlencoded";

            requete.BeginGetRequestStream(DebutReponse, requete);
            //}
            //else
            //    MessageBox.Show("Please fill all the fields");
        }

        #region Post Requete handling

        private void DebutReponse(IAsyncResult resultatAsynchrone)
        {
            Dispatcher.BeginInvoke(() => this.loaderGrid.Visibility = Visibility.Visible);
            HttpWebRequest requete = (HttpWebRequest)resultatAsynchrone.AsyncState;
            if (requete != null)
            {
                try
                {
                    Dispatcher.BeginInvoke(() =>
                    {

                        Stream postStream = requete.EndGetRequestStream(resultatAsynchrone);
                        //string postData = string.Format("Email={0}&Password={1}", user.login, user.password);
                        string postData = string.Format("Email=test@test.com&Password=testes");

                        byte[] tableau = Encoding.UTF8.GetBytes(postData);
                        postStream.Write(tableau, 0, postData.Length);
                        postStream.Close();
                        requete.BeginGetResponse(FinReponse, requete);
                    });
                }
                catch (WebException ex)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(ex.Message));
                }
            }
        }

        private void FinReponse(IAsyncResult resultatAsynchrone)
        {
            HttpWebRequest requete = (HttpWebRequest)resultatAsynchrone.AsyncState;

            if (requete != null)
            {
                try
                {
                    WebResponse webResponse = requete.EndGetResponse(resultatAsynchrone);
                    Stream stream = webResponse.GetResponseStream();
                    StreamReader streamReader = new StreamReader(stream);
                    string reponse = streamReader.ReadToEnd();

                    stream.Close();
                    streamReader.Close();
                    webResponse.Close();
                    Dispatcher.BeginInvoke(() =>
                    {
                        user.token = reponse.Replace("\"", "");
                        //await saveUserDetailAsync(); // sa plante apres 3 appel a verifier

                        PhoneApplicationService.Current.State["User"] = user;
                        NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
                    });
                }
                catch (WebException ex)
                {
                    string reponse = null;
                    StreamReader sr = new StreamReader(ex.Response.GetResponseStream(), true);

                    if (sr != null)
                        reponse = sr.ReadToEnd();

                    if (reponse != null && reponse != "")
                        showJsonError(reponse);
                    else
                        Dispatcher.BeginInvoke(() => MessageBox.Show(ex.Message));
                }

            }
            Dispatcher.BeginInvoke(() => this.loaderGrid.Visibility = Visibility.Collapsed);
        }

        private void showJsonError(string reponse)
        {
            string errorString = "";
            JObject json = (JObject)JObject.Parse(reponse);

            if (json["ModelState"] != null)
            {
                if (json["ModelState"]["model.Email"] != null)
                {
                    errorString = (string)json["ModelState"]["model.Email"][0];
                }
                if (json["ModelState"]["model.Password"] != null)
                {
                    errorString += "\n" + (string)json["ModelState"]["model.Password"][0];
                }
            }
            else if (json["Message"] != null)
                errorString = (string)json["Message"];
            else
                errorString = "An error occur while log in please try again.";
            Dispatcher.BeginInvoke(() => MessageBox.Show(errorString));
        }

        #endregion

        #region Create account

        private void Create_Account_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Auth/CreateAccount.xaml", UriKind.Relative));
        }

        #endregion
    }
}