using okboba.Entities;
using okboba.MatchCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace okboba.MatchApi.Controllers
{
    [Authorize]
    public class AnswerController : OkbBaseController
    {
        //// Private Variables
        private MatchCalc _matchCalc;

        //// Constructor
        public AnswerController()
        {
            _matchCalc = MatchCalc.Instance;
        }

        /// <summary>
        /// GET: /api/answer/count
        /// get the number of questions answered for the given user
        /// </summary>
        [HttpGet]
        public int Count(int id)
        {
            var answers = _matchCalc.GetAnswers(id);
            if (answers != null)
            {
                return answers.Count();
            }
            return 0;
        }

        /// <summary>
        /// GET: /api/answer/intersection
        /// Get all of P1's answers that is an intersection with p2's answers.
        /// </summary>
        [HttpGet]
        public IDictionary<short,Answer> Intersection(int p1, int p2)
        {
            var ansDict = _matchCalc.GetAnswerDict(p2);
            var ansList = _matchCalc.GetAnswers(p1);
            var dict = new Dictionary<short, Answer>();

            if (ansList == null) return dict;

            foreach (var ans in ansList)
            {
                if (ansDict.ContainsKey(ans.QuestionId))
                {
                    dict.Add(ans.QuestionId, new Answer
                    {
                        QuestionId = ans.QuestionId,
                        ProfileId = p1,
                        ChoiceIndex = _matchCalc.ChoiceIndex(ans.ChoiceBit),
                        ChoiceAccept = ans.ChoiceAccept,
                        ChoiceWeight = ans.ChoiceWeight,
                        LastAnswered = ans.LastAnswered
                    });
                }
            }

            return dict;
        }

        // POST api/answer
        /// <summary>
        /// Updates/Adds the users answer in the answer cache.
        /// </summary>
        public void Post([FromBody]Answer answer)
        {
            //We should be authenticated at this point.  Only allow users to update their own answers
            var profileId = GetProfileId();

            answer.ProfileId = profileId;

            //we don't do any validation since all of it is done upstream before we get here.
            _matchCalc.AddOrUpdate(answer);
        }
    }
}