using okboba.Entities;
using okboba.Repository.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository
{
    public interface IQuestionRepository
    {
        Task AnswerAsync(Answer ans);
        IPagedList<QuestionAnswerModel> GetQuestions(int profileId, int page = 1, int perPage = 10);
        Dictionary<short, Answer> GetAnswers(int profileId);
        IList<QuestionModel> Next2Questions(int profileId);
        bool ValidateAnswer(Answer ans);
    }
}
