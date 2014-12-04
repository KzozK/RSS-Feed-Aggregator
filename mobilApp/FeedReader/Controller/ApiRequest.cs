using FeedReader.Model;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if (!checkConection())
                return;
            Dispatcher.BeginInvoke(() => this.loaderGrid.Visibility = Visibility.Visible);
            if (NetworkInterface.GetIsNetworkAvailable() == false)
            {
                MessageBox.Show("You don't have internet conection please try later.");
                return;
            }
            try
            {
                HttpWebRequest requete = (HttpWebRequest)HttpWebRequest.Create(url);
                requete.Method = "POST";
                requete.ContentType = "application/x-www-form-urlencoded";

                requete.BeginGetRequestStream(DebutReponse, requete);
            }
            catch (WebException webException)
            {
                MessageBox.Show(webException.Message);
            }
            catch (UriFormatException uriFormatEx)
            {
                MessageBox.Show(uriFormatEx.Message);
            }
        }

        // logout
        private void createTokenRequest(HttpWebRequest requete, IAsyncResult resultatAsynchrone)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Stream postStream = requete.EndGetRequestStream(resultatAsynchrone);
                string postData = string.Format("Token={0}", user.token);

                byte[] tableau = Encoding.UTF8.GetBytes(postData);
                postStream.Write(tableau, 0, postData.Length);
                postStream.Close();
                requete.BeginGetResponse(FinReponse, requete);
            });
        }

        private void createCategoryModificationRequest(HttpWebRequest requete, IAsyncResult resultatAsynchrone)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Stream postStream = requete.EndGetRequestStream(resultatAsynchrone);
                string postData = string.Format("Token={0}&Name={1}&Action={2}&Id={3}", user.token, this.selectedCategory.Name, this.action, this.selectedCategory.Id);
                byte[] tableau = Encoding.UTF8.GetBytes(postData);

                postStream.Write(tableau, 0, tableau.Length);
                postStream.Close();
                requete.BeginGetResponse(FinReponse, requete);
            });
        }

        private void createRssFeedModificationRequest(HttpWebRequest requete, IAsyncResult resultatAsynchrone)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Stream postStream = requete.EndGetRequestStream(resultatAsynchrone);
                string postData = string.Format("Token={0}&Name={1}&Action={2}&Id={3}&CategoryId={4}&URL={5}",
                    user.token, this.selectedRssFeed.Name, this.action, this.selectedRssFeed.Id, this.selectedRssFeed.CategoryId, this.selectedRssFeed.URL);
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
                    if (requete.RequestUri.AbsoluteUri == this.getAllCategoriesUrl || requete.RequestUri.AbsoluteUri == this.getAllRssUrl || requete.RequestUri.AbsoluteUri == this.logoutUrl)
                        createTokenRequest(requete, resultatAsynchrone);
                    else if (requete.RequestUri.AbsoluteUri == this.categoriesManagingUrl)
                        createCategoryModificationRequest(requete, resultatAsynchrone);
                    else if (requete.RequestUri.AbsoluteUri == this.rssManagingUrl)
                        createRssFeedModificationRequest(requete, resultatAsynchrone);
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
                    string response = getStringResponse(requete, resultatAsynchrone);

                    if (requete.RequestUri.AbsoluteUri == this.getAllCategoriesUrl)
                    {
                        sortCategoryResponse(response);
                        getDataFromUrl(this.getAllRssUrl);
                    }
                    else if (requete.RequestUri.AbsoluteUri == this.getAllRssUrl)
                        sortRSSResponse(response);
                    else if (requete.RequestUri.AbsoluteUri == this.categoriesManagingUrl)
                        manageCategory(response);
                    else if (requete.RequestUri.AbsoluteUri == this.rssManagingUrl)
                        manageRss(response);
                    else if (requete.RequestUri.AbsoluteUri == this.logoutUrl)
                        this.logoutEnd();
                    else
                    {
                        addFeed(response);
                        dataManaging.saveCategoryData(this.categoryList);
                    }
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

        private void logoutEnd()
        {
            Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show("Login out successfuly.");
                user = new UserDetail();
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            });
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

        #region managing rss

        private void manageRss(string response)
        {
            JObject jsonObj = JObject.Parse(response);
            RSS res = new RSS()
            {
                Name = (string)jsonObj["Name"],
                Id = (int)jsonObj["Id"],
                UserId = (int)jsonObj["UserId"],
                URL = (string)jsonObj["URL"],
                CategoryId = (int)jsonObj["CategoryId"],
                newsListe = new SyndicationFeed()
            };

            int catIndex = categoryIndexForId(this.categoryList, res.CategoryId);
            int rssFeedIndex = feedIndexForId(this.categoryList.ElementAt(catIndex).rssFeedList, res.Id);

            if (this.action != this.createAction && (catIndex == -1 || rssFeedIndex == -1))
            {
                Debug.WriteLine("Can't find feedIndex to add feed item");
                return;
            }
            Dispatcher.BeginInvoke(() =>
            {
                if (this.action == this.modifAction) // signifie juste modif name
                {
                    this.categoryList.ElementAt(catIndex).rssFeedList.ElementAt(rssFeedIndex).Name = res.Name;
                }
                else if (this.action == this.deleteAction)
                {
                    this.categoryList.ElementAt(catIndex).rssFeedList.RemoveAt(rssFeedIndex);
                }
                else
                    this.categoryList.ElementAt(catIndex).rssFeedList.Add(res);
                manageFeedButton_Click(null, null);
            });
        }

        #endregion

        #region managing Category

        private void manageCategory(string response)
        {

            JObject jsonObj = JObject.Parse(response);
            Category res = new Category()
            {
                Name = (string)jsonObj["Name"],
                Id = (int)jsonObj["Id"],
                UserId = (int)jsonObj["UserId"],
                rssFeedList = new ObservableCollection<RSS>()
            };

            int index = categoryIndexForId(this.categoryList, res.Id);

            Dispatcher.BeginInvoke(() =>
            {
                if (this.action == this.modifAction) // signifie juste modif name
                {
                    this.categoryList.ElementAt<Category>(index).Name = (string)jsonObj["Name"];
                }
                else if (this.action == this.deleteAction)
                {
                    this.categoryList.RemoveAt(index);
                }
                else
                    this.categoryList.Add(res);
                MessageBox.Show("The modification are done");
                manageCategoryButton_Click(null, null);
            });
        }

        #endregion

        #region RSS FEED LIST

        private void sortRssFeed(IEnumerable<RSS> feedList)
        {
            foreach (Category item in this.categoryList)
            {
                item.rssFeedList = new ObservableCollection<RSS>(feedList.Where(rss => rss.CategoryId == item.Id));
            }
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
                        URL = item.ToObject<RSS>().URL,
                        newsListe = new SyndicationFeed()
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
                    dataManaging.saveRssFeed(feed, currentCat.rssFeedList.ElementAt(this.selectedFeedIndex).Name);
                });
            }
            catch (XmlException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        #endregion

        #region CATEGORY

        private void sortCategoryResponse(string response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                JArray jsonArray = JArray.Parse(response);

                foreach (var item in jsonArray)
                {
                    this.categoryList.Add(new Category()
                    {
                        Id = item.ToObject<Category>().Id,
                        Name = item.ToObject<Category>().Name,
                        UserId = item.ToObject<Category>().UserId,
                        rssFeedList = new ObservableCollection<RSS>()
                    });
                }
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
