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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FeedReader
{

    public partial class MainPage : PhoneApplicationPage
    {
        // DEFINE
        private string getAllCategoriesUrl = "http://rssfeedagregator.azurewebsites.net/api/categories/categories";
        private string categoriesManagingUrl = "http://rssfeedagregator.azurewebsites.net/api/categories";
        private string getAllRssUrl = "http://rssfeedagregator.azurewebsites.net/api/rss/rss";
        private string rssManagingUrl = "http://rssfeedagregator.azurewebsites.net/api/rss";
        private string createAction = "POST";
        private string deleteAction = "DELETE";
        private string modifAction = "PUT";

        //Category & rss managing property
        private string action = "";
        private Category selectedCategory = null;
        private RSS selectedRssFeed = null;

        ObservableCollection<Category> categoryList = new ObservableCollection<Category>();
        int downloadingCategoryId = 0;
        int selectedFeedIndex = 0;
        UserDetail user = new UserDetail();


        // Constructor
        public MainPage()
        {
            InitializeComponent();
            SliderView.SelectedIndex = 1;
            this.action = this.createAction;
            this.CategoryLLS.ItemsSource = this.categoryList;
            this.getDataFromUrl(this.getAllCategoriesUrl);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            user = (UserDetail)PhoneApplicationService.Current.State["User"];
            base.OnNavigatedTo(e);
        }
        private void SlideView_OnSelectionChanged(object sender, EventArgs e)
        {
        }

        #region Search Func

        private Category findCategoryByName(string name)
        {
            return this.categoryList.First<Category>(cat => cat.Name == name);
        }
        private Category findCategoryById(int id)
        {
            return this.categoryList.First<Category>(cat => cat.Id == id);
        }

        private RSS findFeedInCategoryByName(Category cat, string name)
        {
            return cat.rssFeedList.First(rss => rss.Name == name);
        }

        private static int categoryIndexForId(ObservableCollection<Category> source, int id)
        {
            int index = 0;
            foreach (Category item in source)
            {
                if (item.Id == id)
                    return index;
                index++;
            }
            return -1;
        }

        private static int feedIndexForId(ObservableCollection<RSS> source, int id)
        {
            int index = 0;
            foreach (RSS item in source)
            {
                if (item.Id == id)
                    return index;
                index++;
            }
            return -1;
        }

        #endregion

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

        #region MANAGE CATEGORY

        private void manageCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility visibilityParam = (this.categoryManagerGrid.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed);
            this.MaskGrid.Visibility = visibilityParam;
            this.categoryManagerGrid.Visibility = visibilityParam;
            this.categoryManageGridlistPicker.ItemsSource = this.categoryList.Select(cat => cat.Name).ToList<string>();
            this.catNameBox.Text = "";
            this.action = "";
            this.selectedCategory = null;
        }

        private void createCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.catNameBox.Text == "")
            {
                MessageBox.Show("Please write a name");
                return ;
            }
            this.selectedCategory = new Category() { Name = this.catNameBox.Text, Id = 0};
            this.action = this.createAction;

            this.getDataFromUrl(this.categoriesManagingUrl);
        }
        private void modifyCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.categoryList.Count == 0)
            {
                MessageBox.Show("There is no category to Modify.");
                return;
            }
            if (this.catNameBox.Text == "")
            {
                MessageBox.Show("Please write the new name");
                return;
            }
            this.action = this.modifAction;
            this.selectedCategory = findCategoryByName((string)this.categoryManageGridlistPicker.SelectedItem);
            this.selectedCategory.Name = this.catNameBox.Text;

            this.getDataFromUrl(this.categoriesManagingUrl);
        }
        private void deleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.categoryList.Count == 0)
            {
                MessageBox.Show("There is no category to delete.");
                return;
            }
            this.action = this.deleteAction;
            this.selectedCategory = findCategoryByName((string)this.categoryManageGridlistPicker.SelectedItem);

            this.getDataFromUrl(this.categoriesManagingUrl);
        }

        #endregion

        #region MANAGE FEED


        private void manageFeedButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.categoryList.Count == 0)
                return;
            Visibility visibilityParam = (this.rssFeedManageGrid.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed);
            this.MaskGrid.Visibility = visibilityParam;
            this.rssFeedManageGrid.Visibility = visibilityParam;
            this.categorylistPickerForRss.ItemsSource = this.categoryList.Select(cat => cat.Name).ToList<string>();
            this.rssFeedNameBox.Text = "";
            this.rssFeedURLBox.Text = "";
            this.selectedRssFeed = null;
            this.action = "";
            Category selectedCat = findCategoryByName((string)this.categorylistPickerForRss.SelectedItem);
            this.rssFeedManageGridlistPicker.ItemsSource = selectedCat.rssFeedList.Select(rss => rss.Name);
        }

        private void categorylistPickerForRss_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category selectedCat = findCategoryByName((string)this.categorylistPickerForRss.SelectedItem);
            this.rssFeedManageGridlistPicker.ItemsSource = selectedCat.rssFeedList.Select(rss => rss.Name);
        }

        private void createRssFeedButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.rssFeedNameBox.Text == "" || this.rssFeedURLBox.Text == "")
            {
                MessageBox.Show("Please fill all field");
                return;
            }
            this.selectedRssFeed = new RSS();
            this.selectedRssFeed.Id = 0;
            this.selectedRssFeed.CategoryId = findCategoryByName((string)this.categorylistPickerForRss.SelectedItem).Id;
            this.selectedRssFeed.Name = this.rssFeedNameBox.Text;
            this.selectedRssFeed.URL = this.rssFeedURLBox.Text;
            this.action = this.createAction;

            this.getDataFromUrl(this.rssManagingUrl);
        }
        private void modifyRssFeedButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.rssFeedManageGridlistPicker.Items.Count == 0)
            {
                MessageBox.Show("There is no feed to modify.");
                return;
            }
            if (this.rssFeedNameBox.Text == "" || this.rssFeedURLBox.Text == "")
            {
                MessageBox.Show("Please fill the Name and Url field");
                return;
            }

            Category cat = findCategoryByName((string)this.categorylistPickerForRss.SelectedItem);
            this.selectedRssFeed = findFeedInCategoryByName(cat, (string)this.rssFeedManageGridlistPicker.SelectedItem);
            this.selectedRssFeed.Name = (this.rssFeedNameBox.Text != "") ? this.rssFeedNameBox.Text : this.selectedRssFeed.Name;
            this.selectedRssFeed.URL = (this.rssFeedURLBox.Text != "") ? this.rssFeedURLBox.Text : this.selectedRssFeed.URL;
            this.action = this.modifAction;

            this.getDataFromUrl(this.rssManagingUrl);
        }
        private void deleteRssFeedButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.rssFeedManageGridlistPicker.Items.Count == 0)
            {
                MessageBox.Show("There is no feed to delete.");
                return;
            }
            this.action = this.deleteAction;
            Category cat = findCategoryByName((string)this.categorylistPickerForRss.SelectedItem);
            this.selectedRssFeed = findFeedInCategoryByName(cat, (string)this.rssFeedManageGridlistPicker.SelectedItem);
            this.action = this.deleteAction;

            this.getDataFromUrl(this.rssManagingUrl);
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

            if (item == null)
                return;
            PhoneApplicationService.Current.State["FeedItem"] = item;
            NavigationService.Navigate(new Uri("/View/FeedDetailPage.xaml", UriKind.Relative));
        }
        #endregion

        #endregion

        #region ACCOUNT
        private void accountButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/AccountSettingPage.xaml", UriKind.Relative));
        }
        #endregion
    }
}