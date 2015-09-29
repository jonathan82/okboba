using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.MatchLoader.Models
{
    public class UserInMem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public char Gender { get; set; }
        public List<AnswerInMem> Answers { get; set; }
    }
}
