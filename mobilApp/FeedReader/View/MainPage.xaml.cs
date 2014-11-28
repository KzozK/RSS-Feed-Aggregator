using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.UI.ViewManagement;
using FeedReader.Resources;
using FeedReader.Model;
using SlideView.Library;

using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.ServiceModel.Syndication; // parser XML pour les flux rss
using Windows.ApplicationModel;
using System.IO;
using System.Collections;

namespace FeedReader
{
    public partial class MainPage : PhoneApplicationPage
    {
        WebClient client = new WebClient();
        IEnumerable<Category> categoryList = new List<Category>();
        int downloadingCategoryId = 0;
        int selectedFeedIndex = 0;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            SliderView.SelectedIndex = 1;

            // lecture d'un flux rss (XML)
            client.DownloadStringCompleted += client_DownloadStringCompleted;
            client.DownloadStringAsync(new Uri("http://rssfeedagregator.azurewebsites.net/api/categories"), "CategoryRequest");


            this.categoryList = new List<Category>
        {
            this.createCategoryModel("Jeux", 0, this.ObtientImage(1)),
            this.createCategoryModel("Sport", 1, this.ObtientImage(3)),
            this.createCategoryModel("Culture", 2, this.ObtientImage(2))
        };

            CategoryLLS.ItemsSource = this.categoryList.ToList();
        }

        // read rss feed of a website
        private void addFeed(string flux)
        {
            StringReader stringReader = new StringReader(flux);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
            Category currentCat = this.categoryList.First(cat => cat.Id == this.downloadingCategoryId);

            currentCat.rssFeedList.ElementAt(this.selectedFeedIndex).newsListe = feed;
            NewsListBox.ItemsSource = feed.Items;
        }

        private void sortRssFeed(IEnumerable<RSS> feedList)
        {
            foreach (Category item in this.categoryList)
            {
                item.rssFeedList = feedList.Where(rss => rss.CategoryId == item.Id);
            }
        }

        private void rssFeedRequestCompleted(XDocument loadedData)
        {
            IEnumerable<RSS> feedList =
                from query in loadedData.Descendants("RSS")
                select new RSS
                {
                    Id = (int)query.Element("Id"),
                    URL = (string)query.Element("URL"),
                    Flux = (string)query.Element("Flux"),
                    CategoryId = (int)query.Element("CategoryId"),
                    UserId = (int)query.Element("UserId"),
                    image = new BitmapImage(new Uri("/Assets/Images/green.png", UriKind.Relative)),
                    numberOfFeed = 17
                };
            this.sortRssFeed(feedList);
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if ((string)e.UserState == "CategoryRequest")
                {
                    XDocument loadedData = XDocument.Load(e.Result);
                    this.categoryList =
                        from query in loadedData.Descendants("Category")
                        select new Category
                        {
                            Id = (int)query.Element("id"),
                            Name = (string)query.Element("Name"),
                            UserId = (int)query.Element("UserId")
                        };
                    client.DownloadStringAsync(new Uri("http://rssfeedagregator.azurewebsites.net/api/rss"), "RssFeedListRequest");
                }
                else if ((string)e.UserState == "RssFeedListRequest")
                {
                    XDocument loadedData = XDocument.Load(e.Result);

                    rssFeedRequestCompleted(loadedData);
                }
                else if ((string)e.UserState == "FeedRequest")
                {
                    this.addFeed(e.Result);
                }
            }
            else
            {
                MessageBox.Show("Impossible de récupérer les données sur internet : " + e.Error);
            }
        }


        private Category createCategoryModel(string name, int catId, BitmapImage img)
        {
            return new Category
            {
                Id = catId,
                Name = name,
                UserId = 93,
                rssFeedList = new List<RSS> { this.createFeedModel("test feed name", catId, img), this.createFeedModel("test 2", catId, img), this.createFeedModel("test 3", catId, img) }
            };
        }

        private RSS createFeedModel(string name, int catId, BitmapImage img)
        {
            return new RSS {
                CategoryId = catId,
                Flux = "Je suis un flux",
                Id = 123,
                image = new BitmapImage(new Uri("/Assets/Images/green.png", UriKind.Relative)),
                numberOfFeed = 117,
                URL = "http://www.gameblog.fr/rss.php",
                UserId = 93
            };
        }

        private BitmapImage ObtientImage(int priorite)
        {
            if (priorite <= 1)
                return new BitmapImage(new Uri("/Assets/Images/green.png", UriKind.Relative));
            return new BitmapImage(new Uri("/Assets/Images/red.png", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        private void SlideView_OnSelectionChanged(object sender, EventArgs e)
        {
        }

        #region FEED VIEW

        #region Category ListBox

        private void RssListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // cancel previous request
            client.CancelAsync();
            
            ListBox tmp = (ListBox)sender;
            RSS rss = (RSS)tmp.SelectedItem;

            selectedFeedIndex = tmp.SelectedIndex;
            downloadingCategoryId = rss.CategoryId;

            // when selected do request
            client.DownloadStringAsync(new Uri(rss.URL), "FeedRequest");
        }

        #endregion

        #region LongListSelector

        private void CategoryLLS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Debug.WriteLine();
            // CategoryLLS.Visibility
        }

        #endregion

        #region NEWS VIEW

        private void NewsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        #endregion
        #endregion
    }
}