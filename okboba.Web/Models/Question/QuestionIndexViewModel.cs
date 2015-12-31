using okboba.Entities;
using okboba.Repository.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class QuestionIndexViewModel
    {
        public string UserId { get; set; }
        public int ProfileId { get; set; }
        public bool IsMe { get; set; }
        public IPagedList<QuestionAnswerModel> Questions { get; set; }
        public Dictionary<short, Answer> CompareQuestions { get; set; }
        public IEnumerable<QuestionModel> NextQuestions { get; set; }
        public Profile Profile { get; set; }
        public Profile CompareProfile { get; set; }
        //public string AvatarSmall { get; set; }
        //public string AvatarCompareSmall { get; set; }
        //public byte Gender { get; set; }
        //public byte GenderCompare { get; set; }
        //public string UserIdCompare { get; set; }
    }
}