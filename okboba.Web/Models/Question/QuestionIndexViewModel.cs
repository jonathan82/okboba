using okboba.Repository.Models;
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
        public IEnumerable<QuestionWithAnswerModel> MyQuestions { get; set; }
    }
}