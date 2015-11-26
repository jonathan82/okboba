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

        /// <summary>
        /// Gets a list of all the answer choices for a given question
        /// </summary>
        private IEnumerable<QuestionChoice> GetChoices(int id)
        {
            var db = new OkbDbContext();
            var list = new List<QuestionChoice>();
            var result = from choice in db.QuestionChoices.AsNoTracking()
                         where choice.QuestionId == id
                         orderby choice.Index ascending
                         select choice;

            foreach (var choice in result)
            {
                list.Add(choice);
            }

            return list;
        }

        /// <summary>
        /// Gets the next 2 un-answered questions for the user sorting by rank. Builds 
        /// a dcitionary of answers for the user to speed up lookup. 
        /// </summary>
        public IEnumerable<QuestionWithAnswerModel> GetNext2Questions(int profileId)
        {
            var db = new OkbDbContext();

            //Build answer dictionary
            var result = from answer in db.Answers.AsNoTracking()
                         where answer.ProfileId == profileId
                         select answer;

            var set = new HashSet<Int16>();

            foreach (var answer in result)
            {
                set.Add(answer.QuestionId);
            }

            var list = new List<QuestionWithAnswerModel>();

            int count = 0;

            //Loop thru all the questions
            foreach (var question in db.Questions.AsNoTracking())
            {
                if (set.Contains(question.Id)) continue;
                list.Add(new QuestionWithAnswerModel
                {
                    Question = question,
                    Choices = GetChoices(question.Id)
                });

                count++;

                if (count >= 2) break; //just get the next 2 questions
            }

            return list;
        }

        /// <summary>
        /// Get a list of all the answered questions for the given profile ID sorted by
        /// the date last answered from the database.
        /// </summary>
        public IEnumerable<QuestionWithAnswerModel> GetAnsweredQuestions(int profileId)
        {       
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
                ((List<QuestionChoice>)list[list.Count - 1].Choices).Add(new QuestionChoice
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
