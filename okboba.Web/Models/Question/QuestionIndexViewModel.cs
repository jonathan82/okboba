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
        public int ProfileId { get; set; }
        public bool IsMe { get; set; }
        public IPagedList<QuestionAnswerModel> Questions { get; set; }
        public Dictionary<short, Answer> CompareQuestions { get; set; }
        public IEnumerable<QuestionModel> NextQuestions { get; set; }
    }
}