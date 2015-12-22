using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.Models
{
    public class QuestionAnswerModel
    {
        public QuestionModel Question { get; set; }
        public Answer Answer { get; set; }
    }
}
