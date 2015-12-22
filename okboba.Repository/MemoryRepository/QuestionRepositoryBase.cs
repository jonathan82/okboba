using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.MemoryRepository
{
    public class QuestionRepositoryBase
    {
        /// <summary>
        /// Validates an answer by making sure it follows the following conventions:
        /// 
        ///     - Skipped: ChoiceBit = null and the rest 0
        ///     - Irrelevant: ChoiceWeight = 0 and all bits set to 1 on ChoiceAcceptable
        /// 
        /// </summary>
        protected Answer ValidateAnswer(Answer ans)
        {
            if (ans.ChoiceIndex == null || ans.ChoiceIndex == 0)
            {
                //skipping question
                ans.ChoiceIndex = null;
                ans.ChoiceWeight = 0;
                ans.ChoiceAccept = 0;
            }
            else
            {
                //validate answer
                if (ans.ChoiceWeight == 0)
                {
                    // user chose irrelevant, mark all answers as acceptable
                    ans.ChoiceAccept = 0xFF;
                }
            }

            return ans;
        }

        /// <summary>
        /// 
        /// Updates/Adds a users answer in the database. Called by AnswerQuestion which already
        /// validated the answer so we don't have to here.
        /// 
        /// </summary>
        protected void AnswerQuestionDb(Answer ans)
        {
            var db = new OkbDbContext();

            var dbAns = db.Answers.Find(ans.ProfileId, ans.QuestionId);

            //Are we updating a question or adding a new one?
            if (dbAns == null)
            {
                //new answer
                ans.LastAnswered = DateTime.Now;
                db.Answers.Add(ans);
            }
            else
            {
                //update answer
                dbAns.ChoiceIndex = ans.ChoiceIndex;
                dbAns.ChoiceWeight = ans.ChoiceWeight;
                dbAns.ChoiceAccept = ans.ChoiceAccept;
                dbAns.LastAnswered = DateTime.Now;
            }

            db.SaveChanges();
        }
    }
}
