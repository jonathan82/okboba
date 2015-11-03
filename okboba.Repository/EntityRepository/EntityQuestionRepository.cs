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

        /// <summary>
        /// Answer a question by adding it to the Answers table and updates the user's
        /// current question.
        /// </summary>
        public void AnswerQuestion(int profileId, int questionId, int answerIndex, bool[] answerAccept, int weight, int nextQuestionId)
        {
            var db = new OkbDbContext();

            var encAccept = convertBoolToAccept(answerAccept);

            var answer = new Answer
            {
                QuestionId = (short)questionId,
                ChoiceIndex = (byte)answerIndex,
                ChoiceAcceptable = encAccept,
                ChoiceWeight = (byte)weight,
                ProfileId = profileId,
                LastAnswered = DateTime.Now
            };

            // Transaction - implicit due to DbContext
            // Add answer to database
            db.Answers.Add(answer);

            // Update user's current question to next one
            var user = db.Profiles.Find(profileId);
            user.CurrentQuestionId = (short)nextQuestionId;

            db.SaveChanges();
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
