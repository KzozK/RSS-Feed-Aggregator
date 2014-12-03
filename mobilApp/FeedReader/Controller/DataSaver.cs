using FeedReader.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Serialization;
using Windows.Storage;

namespace FeedReader
{
    public class DataSaver
    {

        #region load & Save Login
        public void saveLoginData(UserDetail user)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            string path = string.Concat(local.Path, "\\user.xml");

            try
            {
                XmlSerializer ser = new XmlSerializer(user.GetType());

                FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                ser.Serialize(fs, user);
            }
            catch (Exception)
            {
                MessageBox.Show("An error occure please try when your connection is back");
            }
        }

        public UserDetail loadLoginData()
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            string path = string.Concat(local.Path, "\\user.xml");
            UserDetail user = new UserDetail();

            try
            {
                XmlSerializer ser = new XmlSerializer(user.GetType());

                FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);
                user = (UserDetail)ser.Deserialize(fs);
                return user;
            }
            catch (Exception)
            {
                MessageBox.Show("An error occure please try when your connection is back");
            }
            return user;
        }
        #endregion

        #region load & Save category
        public void saveCategoryData(ObservableCollection<Category> list)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            string path = string.Concat(local.Path, "\\category.xml");

            try
            {
                XmlSerializer ser = new XmlSerializer(list.GetType());

                FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                ser.Serialize(fs, list);
            }
            catch (Exception)
            {
                MessageBox.Show("An error occure please try when your connection is back");
            }
        }

        public ObservableCollection<Category> loadCategoryData()
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            string path = string.Concat(local.Path, "\\category.xml");
            ObservableCollection<Category> catCollection = new ObservableCollection<Category>();

            try
            {
                XmlSerializer ser = new XmlSerializer(catCollection.GetType());

                FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);
                catCollection = (ObservableCollection<Category>)ser.Deserialize(fs);
                return catCollection;
            }
            catch (Exception)
            {
                MessageBox.Show("An error occure please try when your connection is back");
            }
            return catCollection;
        }
        #endregion

        #region load & Save category

        public void saveRssFeed(SyndicationFeed feed, string catName)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            string path = string.Format("{0}\\{1}.xml", local.Path, catName);

            try
            {
                FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                XmlWriter wr = XmlWriter.Create(fs);
                feed.SaveAsRss20(wr);
                wr.Close();
                fs.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("An error occure please try when your connection is back");
            }
        }

        public SyndicationFeed loadRssFeed(string catName)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            string path = string.Format("{0}\\{1}.xml", local.Path, catName);
            SyndicationFeed feed = new SyndicationFeed();

            try
            {
                FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);
                XmlReader wr = XmlReader.Create(fs);
                feed = SyndicationFeed.Load(wr);
                wr.Close();
                fs.Close();
                return feed;
            }
            catch (Exception)
            {
                MessageBox.Show("An error occure please try when your connection is back");
            }
            return new SyndicationFeed();
        }
        #endregion
    }
}
