using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.MatchLoader.Models
{
    public class AnswerInMem
    {
        public Int16 QuestionId { get; set; }
        public byte ChoiceIndex { get; set; }
        public byte ChoiceAcceptable { get; set; }
        public byte ChoiceWeight { get; set; }
    }
}
