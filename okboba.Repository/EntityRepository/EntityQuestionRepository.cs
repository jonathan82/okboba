using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            ques.Choices = new List<string>();

            //Get the internal representation of choices in the database
            if (ques.ChoicesInternal != null)
            {
                var choices = ques.ChoicesInternal.Split(';');
                foreach (var c in choices)
                {
                    ques.Choices.Add(c);
                }
            }
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
        /// Converts an array of booleans representing acceptable choices
        /// to a byte representation.
        /// </summary>
        public byte convertBoolToAccept(bool[] bAccept)
        {
            byte encAccept = 0;

            for (int i = 0; i < bAccept.Length; i++)
            {
                encAccept |= (byte)((bAccept[i] ? 1 : 0) << i);
            }
            return encAccept;
        }
    }
}
