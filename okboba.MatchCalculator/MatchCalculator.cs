using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.MatchCalculator
{
    public struct CacheAnswer
    {
        public byte? ChoiceIndex;
        public byte? ChoiceAccept;
        public byte? ChoiceWeight;
        public short QuestionId;
    }

    public class MatchCalc
    {
        #region Singelton        
        private static MatchCalc instance;
        private MatchCalc() { }

        public static MatchCalc Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MatchCalc();
                }
                return instance;
            }
        }
        #endregion        

        /////////////// Member variables ////////////////////
        public const int LITTLE_IMPORTANT = 1;
        public const int SOMEWHAT_IMPORTANT = 5;
        public const int VERY_IMPORTANT = 25;
        private Dictionary<int, List<CacheAnswer>> _answerCache;
        private int[] _weights = { 0, LITTLE_IMPORTANT, SOMEWHAT_IMPORTANT, VERY_IMPORTANT };


        ///////////////// Public Methods //////////////////

        /// <summary>
        /// Calculates the Match Percentage between two users. Takes a dictionary of
        /// answers for one user to speed up finding the intersection. The number of questions both users 
        /// answered (set S) determine the highest match percentage possible according to the 
        /// following fomula: Match Pct = raw Pct - 1/S         
        /// </summary>
        public int CalculateMatchPercent(int profileId, Dictionary<int, Answer> myAnswers)
        {
            int scoreMe = 0,
                scoreThem = 0,
                possibleScoreMe = 0,
                possibleScoreThem = 0,
                s = 0;

            if (!_answerCache.ContainsKey(profileId))
            {
                // User hasn't answered any questions
                return 0;
            }

            foreach (var them in _answerCache[profileId])
            {
                if (!myAnswers.ContainsKey(them.QuestionId)) continue; //I haven't answered this question
                var me = myAnswers[them.QuestionId];
                if (me.ChoiceIndex == null || them.ChoiceIndex == null) continue; // either of us skipped this question
                var meAccept = me.ChoiceAcceptable & them.ChoiceIndex;
                var themAccept = me.ChoiceIndex & them.ChoiceAccept;
                scoreMe += meAccept != 0 ? _weights[(byte)me.ChoiceWeight % _weights.Length] : 0; // modulo to prevent out of bounds
                scoreThem += themAccept != 0 ? _weights[(byte)them.ChoiceWeight % _weights.Length] : 0; // modulo to prevent out of bounds
                possibleScoreMe += _weights[(byte)me.ChoiceWeight % _weights.Length]; // modulo to prevent out of bounds
                possibleScoreThem += _weights[(byte)them.ChoiceWeight % _weights.Length]; // modulo to prevent out of bounds
                s++;
            }

            if (s==0)
            {
                //Users haven't answered any of the same questions
                return 0;
            }

            float pctMe, pctThem;
            pctMe = (float)scoreMe / (float)possibleScoreMe;
            pctThem = (float)scoreThem / (float)possibleScoreThem;

            return (int)(Math.Sqrt(pctMe * pctThem) - (1.0 / (float)s) * 100);
        }

        /// <summary>
        /// Loads the inital answer cache from the database
        /// </summary>
        public void LoadAnswerCache()
        {
            _answerCache = new Dictionary<int, List<CacheAnswer>>();

            var db = new OkbDbContext();

            foreach (var ans in db.Answers.AsNoTracking())
            {
                if (!_answerCache.ContainsKey(ans.ProfileId))
                {
                    _answerCache.Add(ans.ProfileId, new List<CacheAnswer>());
                }

                _answerCache[ans.ProfileId].Add(new CacheAnswer
                {
                    QuestionId = ans.QuestionId,
                    ChoiceIndex = ans.ChoiceIndex,
                    ChoiceAccept = ans.ChoiceAcceptable,
                    ChoiceWeight = ans.ChoiceWeight
                });
            }
        }

        /// <summary>
        /// Gets a user's answer in a dictionary
        /// </summary>
        public Dictionary<int, Answer> GetUserAnswers(int profileId)
        {
            var db = new OkbDbContext();

            var result = from ans in db.Answers.AsNoTracking()
                         where ans.ProfileId == profileId
                         select ans;

            var answers = new Dictionary<int, Answer>();

            foreach (var ans in result)
            {
                answers.Add(ans.QuestionId, ans);
            }

            return answers;
        }
    }
}
