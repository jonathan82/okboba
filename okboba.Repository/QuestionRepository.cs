using okboba.Entities;
using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository
{
    public class QuestionRepository
    {
        #region Singelton
        private static QuestionRepository instance;
        private QuestionRepository() { }

        public static QuestionRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new QuestionRepository();
                }
                return instance;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get the question given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Question GetQuestion(int? id)
        {
            var db = new OkbDbContext();
            var ques = db.Questions.Find(id);

            ques.Choices = new List<string>();

            //Get the internal representation of choices in the database
            if(ques.ChoicesInternal != null)
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
        /// <param name="rank"></param>
        /// <returns></returns>
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

        public List<TranslateQuestionViewModel> GetTranslateQuestions()
        {
            var db = new OkbDbContext();

            var result = from q in db.Questions
                         join qt in db.TranslateQuestions on q.Id equals qt.Id
                         orderby q.Rank ascending
                         select new TranslateQuestionViewModel
                         {
                             Id = q.Id,
                             QuesEng = qt.QuestionText,
                             QuesChin = q.Text,
                             ChoicesEng = qt.ChoicesInternal == null ? null : qt.ChoicesInternal.Split(';'),
                             ChoicesChin = q.ChoicesInternal == null ? null : q.ChoicesInternal.Split(';'),
                             Scores = q.TraitScores == null ? null : q.TraitScores,
                             TraitId = q.TraitId,
                             Rank = q.Rank                             
                         };

            return result.ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Converts an array of booleans representing acceptable choices
        /// to a byte representation.
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public byte convertBoolToAccept(bool[] bAccept)
        {
            byte encAccept = 0;

            for (int i = 0; i < bAccept.Length; i++)
            {
                encAccept |= (byte)((bAccept[i] ? 1 : 0) << i);
            }
            return encAccept;
        }
        #endregion
    }
}
