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
using System.ServiceModel.Syndication;
using Windows.ApplicationModel;
using System.IO;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace FeedReader
{
    public partial class MainPage : PhoneApplicationPage
    {
        private string categoriesUrl = "http://rssfeedagregator.azurewebsites.net/api/categories/categories";
        private string rssUrl = "http://rssfeedagregator.azurewebsites.net/api/rss/rss";

        IEnumerable<Category> categoryList = new List<Category>();
        int downloadingCategoryId = 0;
        int selectedFeedIndex = 0;
        UserDetail user = new UserDetail();


        // Constructor
        public MainPage()
        {
            InitializeComponent();
            SliderView.SelectedIndex = 1;
            this.getDataFromUrl(this.categoriesUrl);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            user = (UserDetail)PhoneApplicationService.Current.State["User"];
            base.OnNavigatedTo(e);
        }
        private void SlideView_OnSelectionChanged(object sender, EventArgs e)
        {
        }

        #region FEED VIEW

        #region Category ListBox

        private void RssListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            RSS rss = (RSS)listBox.SelectedItem;

            if (rss == null)
                return;
            selectedFeedIndex = listBox.SelectedIndex;
            downloadingCategoryId = rss.CategoryId;

            // when selected do request
            listBox.SelectedItem = -1;
            this.CategoryLLS.IsEnabled = false;
            this.getDataFromUrl(rss.URL);
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
            SyndicationItem item = (SyndicationItem)NewsListBox.SelectedItem;
            PhoneApplicationService.Current.State["FeedItem"] = item;
            NavigationService.Navigate(new Uri("/View/FeedDetailPage.xaml", UriKind.Relative));
        }
        #endregion
        #endregion
    }
}