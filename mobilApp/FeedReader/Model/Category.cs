using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace FeedReader.Model
{
    [DataContract]
    public class Category : INotification
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
        private string name;
        [DataMember]
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
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
        private ObservableCollection<RSS> rssfeedlist;
        [DataMember]
        public ObservableCollection<RSS> rssFeedList
        {
            get { return rssfeedlist; }
            set { SetField(ref rssfeedlist, value); }
        }
    }
}