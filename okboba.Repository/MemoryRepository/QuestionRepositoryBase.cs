using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.MemoryRepository
{
    public abstract class QuestionRepositoryBase
    {
        /// <summary>
        /// Makes sure an answer is valid by meeting the following requirements.  Returns true 
        /// if answer is valid, false if not. Has side effect of modifiying passed in Answer.
        /// 
        ///     - Answer isn't skipped (ChoiceIndex is not 0 or null)
        ///     - Set Irrelevant values: If ChoiceWeight = 0 then all bits set to 1 on ChoiceAccept
        ///     - ChoiceWeight must be 0, 1, 2, or 3
        ///     - ChoiceAccept can't be 0
        /// 
        /// </summary>
        public bool ValidateAnswer(Answer ans)
        {
            if (ans.ChoiceIndex == null || ans.ChoiceIndex == 0) return false;

            //Irrelevant
            if (ans.ChoiceWeight == 0)
            {
                // user chose irrelevant, mark all answers as acceptable
                ans.ChoiceAccept = 0xFF;
            }

            if (ans.ChoiceWeight != 0 &&
                ans.ChoiceWeight != 1 &&
                ans.ChoiceWeight != 2 &&
                ans.ChoiceWeight != 3)
            {
                return false;
            }

            if (ans.ChoiceAccept == 0) return false;

            return true;
        }

        /// <summary>
        /// 
        /// Updates/Adds a users answer in the database. Called by AnswerQuestion which already
        /// validated the answer so we don't have to here.
        /// 
        /// </summary>
        protected async Task AnswerDbAsync(Answer ans)
        {
            var db = new OkbDbContext();

            var dbAns = await db.Answers.FindAsync(ans.ProfileId, ans.QuestionId);

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

            await db.SaveChangesAsync();
        }
    }
}
