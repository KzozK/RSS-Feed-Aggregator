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
using SlideView.Library;

using FeedReader.Model;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace FeedReader
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            SliderView.SelectedIndex = 1;


            List<CategoryModel> chosesAFaire = new List<CategoryModel>
        {
            this.createCategoryModel("Jeux", 100, this.ObtientImage(1)),
            this.createCategoryModel("Sport", 7, this.ObtientImage(3)),
            this.createCategoryModel("Culture", 25, this.ObtientImage(2))
        };

            //List<CategoryKeyGroup<CategoryModel>> dataSource = CategoryKeyGroup<CategoryModel>.CreateGroups(new List<string> { "Jeux", "Image", "international", "Culture" });
            CategoryLLS.ItemsSource = chosesAFaire;
        }

        private CategoryModel createCategoryModel(string name, int nbFeed, BitmapImage img)
        {
            return new CategoryModel(name, nbFeed, img, new List<FeedModel> { this.createFeedModel("test feed name", 100, img), this.createFeedModel("test 2", 100, img), this.createFeedModel("test 3", 100, img) });
        }

        private FeedModel createFeedModel(string name, int nbFeed, BitmapImage img)
        {
            FeedModel feed = new FeedModel(name, 100, img);

            return feed;
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

        #region ListBox

        private void FeedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion

        #region LongListSelector

        private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CategoryLLS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Debug.WriteLine();
            // CategoryLLS.Visibility
        }

        #endregion
        #endregion

        #region NEWS VIEW

        private void NewsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        #endregion

    }
}