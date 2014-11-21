using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedReader.Model
{
    class NewsModel
    {
        public string title { get; set; }
        public string desc { get; set; }
        public string author { get; set; }
        public DateTime date { get; set; }
        public string url { get; set; }
    }
}
