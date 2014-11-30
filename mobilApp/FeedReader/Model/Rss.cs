using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.ServiceModel.Syndication;
using System.ComponentModel;

namespace FeedReader.Model
{
    public class RSS : INotification
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        private string url;
        public string URL
        {
            get { return url; }
            set { SetField(ref url, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }

        private int categoryId;
        public int CategoryId
        {
            get { return categoryId; }
            set { SetField(ref categoryId, value); }
        }

        private int userId;
        public int UserId
        {
            get { return userId; }
            set { SetField(ref userId, value); }
        }

        private BitmapImage img;
        public BitmapImage image
                {
            get { return img; }
            set { SetField(ref img, value); }
        }

        private int nbOfFeed;
        public int numberOfFeed
        {
            get { return nbOfFeed; }
            set { SetField(ref nbOfFeed, value); }
        }
        private SyndicationFeed newsliste;
        public SyndicationFeed newsListe
        {
            get { return newsliste; }
            set { SetField(ref newsliste, value); }
        }

    }
}