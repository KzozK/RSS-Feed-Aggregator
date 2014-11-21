using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FeedReader.Model
{
    class FeedModel
    {
        public string name { get; set; }
        public int numberOfFeed { get; set; }
        public BitmapImage image { get; set; }
        public List<NewsModel> newsList { get; set; }


        public FeedModel(string catName, int nbFeed, BitmapImage img, List<NewsModel> nList)
        {
            this.name = catName;
            this.numberOfFeed = nbFeed;
            this.image = img;
            this.newsList = nList;
        }

        public FeedModel(string catName, int nbFeed, BitmapImage img)
        {
            this.name = catName;
            this.numberOfFeed = nbFeed;
            this.image = img;
            this.newsList = new List<NewsModel>();
        }
    }
}
