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
        public IDictionary<short, Answer> CompareQuestions { get; set; }
        public IEnumerable<QuestionModel> NextQuestions { get; set; }
        public Profile Profile { get; set; }
        public Profile CompareProfile { get; set; }
        public int HighestMatchPercent { get; set; }
        public int IntersectionCount { get; set; }
    }
}