using System;
using System.Collections.Generic;
using System.Linq;

namespace FeedReader.Model
{
    public class Category
    {
        public int      Id { get; set; }

        public string   Name { get; set; }

        public int   UserId { get; set; }
        public IEnumerable<RSS> rssFeedList { get; set; }
    }
}