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
            HttpWebRequest requete = (HttpWebRequest)HttpWebRequest.Create("http://rssfeedagregator.azurewebsites.net/api/account/register");
            requete.Method = "POST";
            requete.ContentType = "application/json";


            requete.BeginGetRequestStream(DebutReponse, requete);

            //emailTextBox.Text;
            //confirmPaswordTextBox.text;
            ////paswordTextBox.text;
        }

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
                        //string postData = string.Format("Email=%s&Password=%s&ConfirmPassword=%s", this.emailTextBox.Text, this.paswordTextBox.Text, this.confirmPaswordTextBox.Text);
                        string postData = string.Format("Email=lole@lol.com&Password=tototo&ConfirmPassword=tototo");
                        //string postData = string.Format("Email=lole@lol.com&Password=tototo");

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
                    Dispatcher.BeginInvoke(() => MessageBox.Show(reponse));
                }
                catch (WebException ex)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(ex.Message));

                }

            }
        }
    }
}