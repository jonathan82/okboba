using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.Models
{
    public class QuestionWithAnswerModel
    {
        public Question Question { get; set; }
        public List<QuestionChoice> Choices { get; set; }
        public Answer Answer { get; set; }
    }
}
