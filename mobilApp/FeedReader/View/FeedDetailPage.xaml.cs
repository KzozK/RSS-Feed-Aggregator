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

            if (feed.Title != null)
                this.titleTextBlock.Text = feed.Title.Text;
            if (feed.Authors.Count > 0)
                this.authorTextBlock.Text = feed.Authors.First().Name;
            if (feed.PublishDate != null)
                this.dateTextBlock.Text = feed.PublishDate.Date.ToString();
            if (feed.Summary != null)
                this.descTextBlock.Text = feed.Summary.Text;
            // image dans Summary faire webview 
            //feedImage.Source = new BitmapImage(new Uri(feed.BaseUri.AbsolutePath, UriKind.Absolute));

            base.OnNavigatedTo(e);
        }
    }
}