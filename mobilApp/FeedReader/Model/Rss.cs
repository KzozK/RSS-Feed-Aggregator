using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.ServiceModel.Syndication;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FeedReader.Model
{
    [DataContract]
    public class RSS : INotification
    {
        [DataMember]
        private int id;
        [DataMember]
        public int Id
        {
            get { return id; }
            set { SetField(ref id, value); }
        }
        [DataMember]
        private string url;
        [DataMember]
        public string URL
        {
            get { return url; }
            set { SetField(ref url, value); }
        }
        [DataMember]
        private string name;
        [DataMember]
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }
        [DataMember]
        private int categoryId;
        [DataMember]
        public int CategoryId
        {
            get { return categoryId; }
            set { SetField(ref categoryId, value); }
        }
        [DataMember]
        private int userId;
        [DataMember]
        public int UserId
        {
            get { return userId; }
            set { SetField(ref userId, value); }
        }
        [DataMember]
        private int nbOfFeed;
        [DataMember]
        public int numberOfFeed
        {
            get { return nbOfFeed; }
            set { SetField(ref nbOfFeed, value); }
        }
        [XmlIgnore()]
        private SyndicationFeed newsliste;
        [XmlIgnore()]
        public SyndicationFeed newsListe
        {
            get { return newsliste; }
            set { SetField(ref newsliste, value); }
        }

    }
}