using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Repository.Models
{

    public class TranslateQuestionViewModel
    {
        public int Id { get; set; }
        public string QuesEng { get; set; }
        public string QuesChin { get; set; }
        public string[] ChoicesEng { get; set; }
        public string[] ChoicesChin { get; set; }
        public SByte[] Scores { get; set; }
        public int? TraitId { get; set; }
        public int? Rank { get; set; }
    }
}