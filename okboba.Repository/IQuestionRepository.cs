using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository
{
    public interface IQuestionRepository
    {
        Question GetQuestion(int? id);
        Question GetQuestionByRank(int rank);
        bool AnswerQuestion(Answer ans);
        IQueryable<TranslateQuestion> GetTranslateQuestions();
    }
}
