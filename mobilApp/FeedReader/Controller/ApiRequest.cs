using FeedReader.Model;
using Microsoft.Phone.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Windows;
using System.Xml;

namespace FeedReader
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Create REQUEST
        private void getDataFromUrl(string url)
        {
            HttpWebRequest requete = (HttpWebRequest)HttpWebRequest.Create(url);
            requete.Method = "POST";
            requete.ContentType = "application/x-www-form-urlencoded";

            requete.BeginGetRequestStream(DebutReponse, requete);
        }


        // get all the categories
        private void createCategoryRequest(HttpWebRequest requete, IAsyncResult resultatAsynchrone)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Stream postStream = requete.EndGetRequestStream(resultatAsynchrone);
                string postData = string.Format("Token={0}", user.token);
                byte[] tableau = Encoding.UTF8.GetBytes(postData);

                postStream.Write(tableau, 0, tableau.Length);
                postStream.Close();
                requete.BeginGetResponse(FinReponse, requete);
            });
        }

        // get all the rss feed for all categories
        private void createRssRequest(HttpWebRequest requete, IAsyncResult resultatAsynchrone)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Stream postStream = requete.EndGetRequestStream(resultatAsynchrone);
                string postData = string.Format("Token={0}", user.token);
                byte[] tableau = Encoding.UTF8.GetBytes(postData);

                postStream.Write(tableau, 0, tableau.Length);
                postStream.Close();
                requete.BeginGetResponse(FinReponse, requete);
            });
        }

        #endregion

        #region download Start

        private void DebutReponse(IAsyncResult resultatAsynchrone)
        {
            HttpWebRequest requete = (HttpWebRequest)resultatAsynchrone.AsyncState;
            if (requete != null)
            {
                try
                {
                    if (requete.RequestUri.AbsoluteUri == this.categoriesUrl)
                        createCategoryRequest(requete, resultatAsynchrone);
                    else if (requete.RequestUri.AbsoluteUri == this.rssUrl)
                        createRssRequest(requete, resultatAsynchrone);
                    else
                        requete.BeginGetResponse(FinReponse, requete);
                }
                catch (WebException ex)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(ex.Message));
                }
            }
        }
        #endregion

        #region download Done

        private void FinReponse(IAsyncResult resultatAsynchrone)
        {
            HttpWebRequest requete = (HttpWebRequest)resultatAsynchrone.AsyncState;

            if (requete != null)
            {
                try
                {
                    if (requete.RequestUri.AbsoluteUri == this.categoriesUrl)
                    {
                        sortCategoryResponse(getStringResponse(requete, resultatAsynchrone));
                        getDataFromUrl(this.rssUrl);
                    }
                    else if (requete.RequestUri.AbsoluteUri == this.rssUrl)
                        sortRSSResponse(getStringResponse(requete, resultatAsynchrone));
                    else
                        addFeed(getStringResponse(requete, resultatAsynchrone));
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

        private string getStringResponse(HttpWebRequest requete, IAsyncResult resultatAsynchrone)
        {
            WebResponse webResponse = requete.EndGetResponse(resultatAsynchrone);
            Stream stream = webResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream);
            string response = streamReader.ReadToEnd();

            stream.Close();
            streamReader.Close();
            webResponse.Close();
            return response;
        }

        #region RSS FEED LIST

        private void sortRssFeed(IEnumerable<RSS> feedList)
        {
            foreach (Category item in this.categoryList)
            {
                item.rssFeedList = (IEnumerable<RSS>)(feedList.Where(rss => rss.CategoryId == item.Id).ToList());
            }
            this.CategoryLLS.ItemsSource = this.categoryList.ToList<Category>();// pour raffraichir la liste
        }

        private void sortRSSResponse(string response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                JArray jsonArray = JArray.Parse(response);

                List<RSS> rssList = new List<RSS>();
                foreach (var item in jsonArray)
                {
                    rssList.Add(new RSS()
                    {
                        Id = item.ToObject<RSS>().Id,
                        Name = item.ToObject<RSS>().Name,
                        UserId = item.ToObject<RSS>().UserId,
                        CategoryId = item.ToObject<RSS>().CategoryId,
                        URL = item.ToObject<RSS>().URL
                    });
                }
                sortRssFeed((IEnumerable<RSS>)rssList);
            });
        }


        #endregion

        #region RSS FEED ITEM
        private void addFeed(string flux)
        {
            try
            {
                StringReader stringReader = new StringReader(flux);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
                Category currentCat = this.categoryList.First(cat => cat.Id == this.downloadingCategoryId);

                Dispatcher.BeginInvoke(() =>
                {
                    currentCat.rssFeedList.ElementAt(this.selectedFeedIndex).newsListe = feed;
                    this.NewsListBox.ItemsSource = feed.Items;
                });
            }
            catch (XmlException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Dispatcher.BeginInvoke(() => this.CategoryLLS.IsEnabled = true);

        }
        #endregion

        #region CATEGORY

        private void sortCategoryResponse(string response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                JArray jsonArray = JArray.Parse(response);

                List<Category> catList = new List<Category>();
                foreach (var item in jsonArray)
                {
                    catList.Add(new Category()
                    {
                        Id = item.ToObject<Category>().Id,
                        Name = item.ToObject<Category>().Name,
                        UserId = item.ToObject<Category>().UserId
                    });
                }
                this.categoryList = catList;
                this.CategoryLLS.ItemsSource = this.categoryList.ToList<Category>();
            });
        }

        #endregion

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
