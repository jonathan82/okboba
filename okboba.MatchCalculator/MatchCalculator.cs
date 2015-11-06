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

    public struct MatchResult
    {
        public int MatchPercent;
        public int EnemeyPercent;
        public int FriendPercent;
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
        /// Deletes a user from the answer cache.  For example, if they delete their account.
        /// </summary>
        public void DeleteUser(int profileId)
        {
            if (_answerCache.ContainsKey(profileId))
            {
                _answerCache.Remove(profileId);
            }          
        }

        /// <summary>
        /// Adds or updates a users answer. Creates new user if they're not in the cache,
        /// otherwise update answer.
        /// </summary>        
        public void AddOrUpdate(Answer updateAnswer)
        {
            // Check for new user
            if(!_answerCache.ContainsKey(updateAnswer.ProfileId))
            {
                _answerCache.Add(updateAnswer.ProfileId, new List<CacheAnswer>());
            }

            var listAnswers = _answerCache[updateAnswer.ProfileId];

            //Check if updating or adding new answer
            for(int i=0; i < listAnswers.Count; i++)
            {
                var a = listAnswers[i];

                if(a.QuestionId==updateAnswer.QuestionId)
                {
                    a.ChoiceIndex = updateAnswer.ChoiceIndex;
                    a.ChoiceAccept = updateAnswer.ChoiceAcceptable;
                    a.ChoiceWeight = updateAnswer.ChoiceWeight;

                    //found and updated answer, done
                    return;
                }
            }

            //Got here: New answer
            listAnswers.Add(new CacheAnswer
            {
                QuestionId = updateAnswer.QuestionId,
                ChoiceIndex = updateAnswer.ChoiceIndex,
                ChoiceAccept = updateAnswer.ChoiceAcceptable,
                ChoiceWeight = updateAnswer.ChoiceWeight
            });
        }

        /// <summary>
        /// Calculates the Match, Enemy, and Friend percentage between two users. Takes a dictionary of
        /// answers for one user to speed up finding the intersection. The number of questions both users 
        /// answered (set S) determine the highest match percentage possible according to the 
        /// following fomula: Match Pct = raw Pct - 1/S    
        /// 
        /// Match Percent : Calculate score based on the users answers and their importance rankings.
        /// 
        /// Friend Percent: Calculate score based on similarly answered questions. Disregards importance rankings.
        /// 
        /// Enemy Percent : Calculate score based on how dissimilar users answered their questions.
        ///     case 1 - they answered differently AND either I'm not acceptable to them or they're 
        ///              not acceptable to me: increase enemyScore
        ///     case 2 - they answered same AND importance differs greatly: increase enemyScore
        ///     case 3 - all others don't affect enemyScore
        /// </summary>
        public MatchResult CalculateMatchPercent(int profileId, Dictionary<int, Answer> myAnswers)
        {
            int scoreMe = 0,
                scoreThem = 0,
                possibleScoreMe = 0,
                possibleScoreThem = 0,
                friendScore = 0,
                enemyScore = 0,
                s = 0;

            var result = new MatchResult(); //default values are zero

            if (!_answerCache.ContainsKey(profileId))
            {
                // User hasn't answered any questions
                return result;
            }

            foreach (var them in _answerCache[profileId])
            {
                if (!myAnswers.ContainsKey(them.QuestionId)) continue; //I haven't answered this question
                var me = myAnswers[them.QuestionId];
                if (me.ChoiceIndex == null || them.ChoiceIndex == null) continue; // either of us skipped this question
                bool meAccept = (me.ChoiceAcceptable & them.ChoiceIndex) != 0 ? true : false;
                bool themAccept = (me.ChoiceIndex & them.ChoiceAccept) != 0 ? true : false;
                scoreMe += meAccept ? _weights[(byte)me.ChoiceWeight % _weights.Length] : 0; // modulo to prevent out of bounds
                scoreThem += themAccept ? _weights[(byte)them.ChoiceWeight % _weights.Length] : 0; // modulo to prevent out of bounds
                possibleScoreMe += _weights[(byte)me.ChoiceWeight % _weights.Length]; // modulo to prevent out of bounds
                possibleScoreThem += _weights[(byte)them.ChoiceWeight % _weights.Length]; // modulo to prevent out of bounds
                friendScore += me.ChoiceIndex == them.ChoiceIndex ? 1 : 0;

                //enemy score: more complex
                if( (me.ChoiceIndex != them.ChoiceIndex && (!meAccept || !themAccept)) || //case 1
                    (me.ChoiceIndex==them.ChoiceIndex && Math.Abs((sbyte)me.ChoiceWeight-(sbyte)them.ChoiceWeight) > 1) ) //case 2
                {
                    enemyScore++;
                }

                s++;
            }

            if (s==0)
            {
                //Users haven't answered any of the same questions
                return result;
            }

            float pctMe, pctThem;
            pctMe = (float)scoreMe / possibleScoreMe;
            pctThem = (float)scoreThem / possibleScoreThem;            

            result.MatchPercent = (int)((Math.Sqrt(pctMe * pctThem) - ((float)1 / s)) * 100);
            result.FriendPercent = (int)((float)(friendScore - 1) / s * 100);
            result.EnemeyPercent = (int)((float)(enemyScore - 1) / s * 100);

            // Normalize negative numbers to zero
            result.MatchPercent = result.MatchPercent < 0 ? 0 : result.MatchPercent;
            result.FriendPercent = result.FriendPercent < 0 ? 0 : result.FriendPercent;
            result.EnemeyPercent = result.EnemeyPercent < 0 ? 0 : result.EnemeyPercent;

            return result;
        }

        /// <summary>
        /// Loads the inital answer cache from the database. Returns the number of answers
        /// loaded.
        /// </summary>
        public int LoadAnswerCache()
        {
            int count = 0;
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

                count++;
            }

            return count;
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
