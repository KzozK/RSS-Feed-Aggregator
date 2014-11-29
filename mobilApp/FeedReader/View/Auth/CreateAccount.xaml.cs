using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using FeedReader.Model;

namespace FeedReader.View.Auth
{
    public partial class CreateAccount : PhoneApplicationPage
    {
        public CreateAccount()
        {
            InitializeComponent();


        }

        private void createAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.emailTextBox.Text.Length > 0 && this.paswordTextBox.Text.Length > 0 && this.confirmPaswordTextBox.Text.Length > 0)
            {
                HttpWebRequest requete = (HttpWebRequest)HttpWebRequest.Create("http://rssfeedagregator.azurewebsites.net/api/account/register");
                requete.Method = "POST";
                requete.ContentType = "application/x-www-form-urlencoded";

                requete.BeginGetRequestStream(DebutReponse, requete);
            }
            else
                MessageBox.Show("Please fill all the fields");
        }

        #region Request Handling

        private void DebutReponse(IAsyncResult resultatAsynchrone)
        {
            HttpWebRequest requete = (HttpWebRequest)resultatAsynchrone.AsyncState;
            if (requete != null)
            {
                try
                {
                    Dispatcher.BeginInvoke(() =>
                    {

                        Stream postStream = requete.EndGetRequestStream(resultatAsynchrone);
                        string postData = string.Format("Email={0}&Password={1}&ConfirmPassword={2}", this.emailTextBox.Text, this.paswordTextBox.Text, this.confirmPaswordTextBox.Text);

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
                        MessageBox.Show("Your account is Created\nNow you can login and enjoy our application.");
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();

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
                if (json["ModelState"]["model.ConfirmPassword"] != null)
                {
                    errorString += "\n" + (string)json["ModelState"]["model.ConfirmPassword"][0];
                }
            }
            else if (json["Message"] != null)
                errorString = (string)json["Message"];
            else
                errorString = "An error occur while creating an account please try again.";
            Dispatcher.BeginInvoke(() => MessageBox.Show(errorString));
        }
        #endregion

    }
}