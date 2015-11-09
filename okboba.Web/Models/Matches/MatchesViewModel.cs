using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class MatchesViewModel
    {
        public List<MatchModel> Matches { get; set; }
        public string StorageUrl { get; set; }
        public string MatchApiUrl { get; set; }
        public MatchCriteriaModel MatchCriteria { get; set; }
    }
}