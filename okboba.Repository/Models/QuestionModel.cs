using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.Models
{
    public class QuestionModel
    {
        public short Id { get; set; }
        public string Text { get; set; }
        public IList<string> Choices { get; set; }
    }
}
