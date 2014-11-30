using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FeedReader.Model
{
    public class Category : INotification
    {

        private int id;
        public int Id
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }

        private int userId;
        public int UserId
        {
            get { return userId; }
            set { SetField(ref userId, value); }
        }

        private ObservableCollection<RSS> rssfeedlist;
        public ObservableCollection<RSS> rssFeedList
        {
            get { return rssfeedlist; }
            set { SetField(ref rssfeedlist, value); }
        }
    }
}