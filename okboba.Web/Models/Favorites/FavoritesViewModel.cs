using okboba.Entities;
using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class FavoriteViewModel
    {
        public Profile FavoriteProfile { get; set; }
        public MatchModel MatchInfo { get; set; }
    }
}