using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Windows.Media.Imaging;namespace FeedReader.Model
{
    class CategoryModel
    {
        public string categoryName { get; set; }
        public int numberOfFeed { get; set; }
        public BitmapImage image { get; set; }

        public List<FeedModel> feedList { get; set; }

        public CategoryModel(string catName, int nbFeed, BitmapImage img, List<FeedModel> feedL)
        {
            this.categoryName = catName;
            this.numberOfFeed = nbFeed;
            this.image = img;
            this.feedList = feedL;
        }

        public CategoryModel(string catName, int nbFeed, BitmapImage img)
        {
            this.categoryName = catName;
            this.numberOfFeed = nbFeed;
            this.image = img;
            this.feedList = new List<FeedModel>();
        }
    }
}
