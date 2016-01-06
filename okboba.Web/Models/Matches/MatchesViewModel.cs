using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    /// <summary>
    /// Information that we want to show on a match "card".
    ///  - Avatar photo (crop photo, on hover we shift photo down to show more)
    ///  - Match %, Enemy %
    ///  - Age
    ///  - Location
    ///  - Nickname
    /// </summary>
    public class MatchesViewModel
    {
        public IList<MatchModel> Matches { get; set; }
        public string StorageUrl { get; set; }
        public string MatchApiUrl { get; set; }
        public MatchCriteriaModel MatchCriteria { get; set; }
    }
}