using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedReader.Model;
using Microsoft.Phone.Globalization;
using System.Globalization;

namespace FeedReader
{
    class CategoryKeyGroup<T> : List<T>
    {
        /// <summary>
        /// The delegate that is used to get the key information.
        /// </summary>
        /// <param name="item">An object of type T</param>
        /// <returns>The key value to use for this object</returns>
        public delegate string GetKeyDelegate(T item);

        /// <summary>
        /// The Key of this group.
        /// </summary>
        public string Key { get; private set; }

        public CategoryKeyGroup(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="slg">The </param>
        /// <returns>Theitems source for a LongListSelector</returns>
        private static List<CategoryKeyGroup<T>> CreateGroups(SortedLocaleGrouping slg)
        {
            List<CategoryKeyGroup<T>> list = new List<CategoryKeyGroup<T>>();

            foreach (string key in slg.GroupDisplayNames)
            {
                list.Add(new CategoryKeyGroup<T>(key));
            }

            return list;
        }


        // custom FUNNCCCCCC
        public static List<CategoryKeyGroup<T>> CreateGroups(List<string> names)
        {
            List<CategoryKeyGroup<T>> list = new List<CategoryKeyGroup<T>>();

            foreach (string key in names)
            {
                list.Add(new CategoryKeyGroup<T>(key));
            }

            return list;
        }

        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="items">The items to place in the groups.</param>
        /// <param name="ci">The CultureInfo to group and sort by.</param>
        /// <param name="getKey">A delegate to get the key from an item.</param>
        /// <param name="sort">Will sort the data if true.</param>
        /// <returns>An items source for a LongListSelector</returns>
        public static List<CategoryKeyGroup<T>> CreateGroups(IEnumerable<T> items, List<string> names, GetKeyDelegate getKey, bool sort)
        {
            //SortedLocaleGrouping slg = new SortedLocaleGrouping(ci);
            List<CategoryKeyGroup<T>> list = CreateGroups(names);
            /*
            foreach (T item in items)
            {
                int index = 0;
                if (slg.SupportsPhonetics)
                {
                    //check if your database has yomi string for item
                    //if it does not, then do you want to generate Yomi or ask the user for this item.
                    //index = slg.GetGroupIndex(getKey(Yomiof(item)));
                }
                else
                {
                    index = slg.GetGroupIndex(getKey(item));
                }
                if (index >= 0 && index < list.Count)
                {
                    list[index].Add(item);
                }
            }*/

            return list;
        }

    }
}
