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
        void Answer(Answer ans);
        IPagedList<QuestionAnswerModel> GetQuestions(int profileId, int page = 1, int perPage = 20);
        IList<QuestionModel> Next2Questions(int profileId);
    }
}
