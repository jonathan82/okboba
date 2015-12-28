using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class ProfileDetailViewModel
    {
        public ProfileText ProfileText { get; set; }
        public ProfileDetail ProfileDetail { get; set; }
        //public Dictionary<string,string> DetailDict { get; set; }
        public IDictionary<string,IList<ProfileDetailOption>> DetailOptions { get; set; }
        public bool isMe { get; set; }

        //Helper functions
        public string GetOptionValue(string colName, byte id)
        {
            IList<ProfileDetailOption> options;
            if(DetailOptions.TryGetValue(colName, out options))
            {
                foreach (var option in options)
                {
                    if (option.Id == id) return option.Value;
                }
                //no matching option - return empty string
                return "";
            }

            //if we got here invalid column name
            throw new Exception("Invalid Detail option column name");
        }

        public IEnumerable<ProfileDetailOption> GetOptions(string colName)
        {
            IList<ProfileDetailOption> options;
            if(DetailOptions.TryGetValue(colName, out options))
            {
                return options;
            }

            //if we got here invalid column name
            throw new Exception("Invalid Detail option column name");
        }
    }
}