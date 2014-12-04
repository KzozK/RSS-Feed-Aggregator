using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ServiceModel.Syndication;
using System.Windows.Media.Imaging;

namespace FeedReader.View
{
    public partial class FeedDetailPage : PhoneApplicationPage
    {
        SyndicationItem feed;
        public FeedDetailPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            feed = (SyndicationItem)PhoneApplicationService.Current.State["FeedItem"];
            if (feed == null)
                return;

            if (feed.Title != null) this.titleTextBlock.Text = feed.Title.Text;
            else this.titleTextBlock.Visibility = Visibility.Collapsed;

            if (feed.Authors.Count > 0) this.authorTextBlock.Text = feed.Authors.First().Name;
            else this.authorTextBlock.Visibility = Visibility.Collapsed;

            if (feed.PublishDate != null) this.dateTextBlock.Text = feed.PublishDate.Date.ToString();
            else this.dateTextBlock.Visibility = Visibility.Collapsed;

            //image
            if (feed.Summary != null)
            {
                this.descWebBrowser.NavigateToString(feed.Summary.Text);
            }
            else this.descWebBrowser.Visibility = Visibility.Collapsed;

            base.OnNavigatedTo(e);
        }

        private void descWebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
        }

    }
}