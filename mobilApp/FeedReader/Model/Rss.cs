using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.ServiceModel.Syndication;

namespace FeedReader.Model
{
    public class RSS
    {
        public int      Id { get; set; }

        public string   URL { get; set; }

        public string   Name { get; set; }

        public int      CategoryId { get; set; }
        
        public int   UserId { get; set; }

        public BitmapImage image { get; set; }

        public int numberOfFeed { get; set; }

        public SyndicationFeed newsListe { get; set; }

    }
}