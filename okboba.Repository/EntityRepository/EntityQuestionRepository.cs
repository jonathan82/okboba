using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using okboba.Repository.Models;

namespace okboba.Repository.EntityRepository
{
    public class EntityQuestionRepository : IQuestionRepository
    {
        #region Singelton
        private static EntityQuestionRepository instance;
        private EntityQuestionRepository() { }

        public static EntityQuestionRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityQuestionRepository();
                }
                return instance;
            }
        }
        #endregion

        /// <summary>
        /// Get the question given its ID
        /// </summary>
        public Question GetQuestion(int? id)
        {
            var db = new OkbDbContext();
            var ques = db.Questions.Find(id);

            return ques;
        }

        /// <summary>
        /// Get the question given its rank.
        /// </summary>
        public Question GetQuestionByRank(int rank)
        {
            var db = new OkbDbContext();

            var result = from q in db.Questions.AsQueryable()
                         where q.Rank == rank
                         select q;

            return result.ToList().First();
        }

        private Answer ValidateAnswer(Answer ans)
        {
            if (ans.ChoiceBit == null || ans.ChoiceBit == 0)
            {
                //skipping question
                ans.ChoiceBit = null;
                ans.ChoiceWeight = 0;
                ans.ChoiceAcceptable = 0;
            }
            else
            {
                //validate answer
                if (ans.ChoiceWeight == 0)
                {
                    // user chose irrelevant, mark all answers as acceptable
                    ans.ChoiceAcceptable = 0xFF;
                }
            }

            ans.LastAnswered = DateTime.Now;

            return ans;
        }

        /// <summary>
        /// Answer a question by adding it to the Answers table and updates the user's
        /// current question.
        /// </summary>
        public bool AnswerQuestion(Answer ans)
        {           
            //Are updating a question or adding a new one?
            var db = new OkbDbContext();
            var dbAns = db.Answers.Find(ans.ProfileId, ans.QuestionId);

            if (dbAns==null)
            {
                //new answer
                ans = ValidateAnswer(ans);
                db.Answers.Add(ans);
            }
            else
            {
                //update answer
                //make sure user not updating answer they answered in last 24 hrs
                var diff = DateTime.Now - dbAns.LastAnswered;
                if (diff.Hours < 24) return false;
                ans = ValidateAnswer(ans);
                dbAns.ChoiceBit = ans.ChoiceBit;
                dbAns.ChoiceWeight = ans.ChoiceWeight;
                dbAns.ChoiceAcceptable = ans.ChoiceAcceptable;
                dbAns.LastAnswered = ans.LastAnswered;
            }

            db.SaveChanges();

            return true;
        }

        public IQueryable<TranslateQuestion> GetTranslateQuestions()
        {
            var db = new OkbDbContext();

            var result = from q in db.TranslateQuestions
                         orderby q.Rank ascending
                         select q;

            return result;
        }

        public IEnumerable<QuestionWithAnswerModel> GetAnsweredQuestions(int profileId)
        {
            //select* from answers a join questions q
            //on a.QuestionId = q.Id
            //join QuestionChoices qc
            //on qc.QuestionId = q.id
            //where a.ProfileId = 1000
            //order by q.id asc, qc.[index] asc            

            var db = new OkbDbContext();

            var result = from answer in db.Answers.AsNoTracking()
                         join ques in db.Questions.AsNoTracking()
                         on answer.QuestionId equals ques.Id
                         join qc in db.QuestionChoices.AsNoTracking()
                         on answer.QuestionId equals qc.QuestionId
                         where answer.ProfileId == profileId
                         orderby answer.LastAnswered descending, ques.Id ascending, qc.Index ascending
                         select new 
                         {
                             Question = ques,
                             Choice = qc,
                             Answer = answer
                         };

            var list = new List<QuestionWithAnswerModel>();

            int currQuesId = 0;

            foreach (var item in result)
            {
                if (item.Question.Id != currQuesId)
                {
                    //add new question
                    list.Add(new QuestionWithAnswerModel
                    {
                        Question = item.Question,
                        Answer = item.Answer,
                        Choices = new List<QuestionChoice>()
                    });
                }

                //add the choices
                list[list.Count - 1].Choices.Add(new QuestionChoice
                {
                    Index = item.Choice.Index,
                    Text = item.Choice.Text,
                });

                currQuesId = item.Question.Id;
            }

            return list;
        }
    }
}
