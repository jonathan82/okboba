using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Web.Models
{
    public class AnswerViewModel
    {
        public int QuestionId { get; set; }
        public int ChoiceIndex { get; set; }
        public int[] ChoiceAccept { get; set; }
        public int ChoiceImportance { get; set; }
        public bool ChoiceIrrelevant { get; set; }
    }
}