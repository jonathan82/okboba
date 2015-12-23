//using okboba.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using okboba.MatchCalculator;
//using okboba.Repository.Models;
//using PagedList;

//namespace okboba.Repository.MemoryRepository
//{        
//    /// <summary>
//    /// 
//    /// Repository for answering and retrieving questions.  Stores the Questions in an in-memory data
//    /// structure for fast access. Runs on the MatchAPI server along with the MatchRepository/MatchCalculator
//    /// 
//    /// </summary>
//    public class MemoryQuestionRepository : QuestionRepositoryBase, IQuestionRepository
//    {
//        #region Singelton
//        private static MemoryQuestionRepository instance;
//        private MemoryQuestionRepository()
//        {
//            //Retrieve the MatchCalculator singleton instance
//            _matchCalc = MatchCalc.Instance;
//        }

//        public static MemoryQuestionRepository Instance
//        {
//            get
//            {
//                if (instance == null)
//                {
//                    instance = new MemoryQuestionRepository();
//                }
//                return instance;
//            }
//        }
//        #endregion

//        /////////////////// Private Variables ////////////////////////
//        private List<QuestionModel> _questions;
//        private Dictionary<short, QuestionModel> _questionMap;
//        private MatchCalc _matchCalc;

//        ////////////////// Private Methods //////////////////////////
        
        

//        ////////////////////////// Public Methods /////////////////////////

//        /// <summary>
//        /// 
//        /// Loads the Questions from the database into memory.  Should be called when app starts
//        /// up and when the Questions table changes. Also builds a map of question ID's -> questions
//        /// to speed up lookup.
//        /// 
//        /// </summary>
//        public void LoadQuestions()
//        {
//            var db = new OkbDbContext();

//            _questions = new List<QuestionModel>();
//            _questionMap = new Dictionary<short, QuestionModel>();

//            var query = from q in db.Questions.AsNoTracking()
//                        join c in db.QuestionChoices.AsNoTracking() on q.Id equals c.QuestionId
//                        orderby q.Rank ascending, c.Index ascending
//                        select new
//                        {
//                            Question = q,
//                            Choice = c
//                        };

//            short id = -1;

//            foreach (var row in query)
//            {
//                if (id != row.Question.Id)
//                {
//                    //Add new question
//                    var ques = new QuestionModel
//                    {
//                        Id = row.Question.Id,
//                        Text = row.Question.Text,
//                        Choices = new List<string>()
//                    };

//                    _questions.Add(ques);
//                    _questionMap.Add(ques.Id, ques);

//                    id = ques.Id;
//                }

//                //Add Choice to existing question
//                _questions[_questions.Count - 1].Choices.Add(row.Choice.Text);
//            }
//        }

//        /// <summary>
//        /// 
//        /// Updates/Adds a user's answer in the Cache as well as DB. Responsible for keeping DB and cache in sync.
//        /// We don't need to check for 24hr condition since the frontend will already disallow this. No need to be
//        /// too strict about this.
//        /// 
//        ///     - Wraps the operation in a transaction so in case something goes wrong DB can be rolled back.  
//        ///     - Validates users answer by calling ValidateAnswer   
//        ///     - Uses MatchCalculator to update/add in memory answer
//        ///     - Calls AnswerQuestionDb to perform database operation
//        /// 
//        /// Note: The order of operations matter. First we update the database, then memory. If something goes
//        ///       wrong we can rollback the database, but not possible for memory.
//        /// 
//        /// </summary>
//        public void Answer(Answer ans)
//        {
//            ans = ValidateAnswer(ans);
//            AnswerQuestionDb(ans);
//            _matchCalc.AddOrUpdate(ans);
//        }

//        /// <summary>
//        /// 
//        /// Gets a list questions answered by the given user, sorted by the date last answered.
//        /// Supports paging.
//        /// 
//        /// </summary>
//        public IPagedList<QuestionAnswerModel> GetQuestions(int profileId, int page = 1, int perPage = 20)
//        {
//            //Get users answers and create a copy since we will modify it (sort)
//            var answers = new List<CacheAnswer>(_matchCalc.GetAnswers(profileId));                      

//            //Sort by last answered date descending
//            answers.Sort(delegate (CacheAnswer x, CacheAnswer y)
//            {
//                return y.LastAnswered.CompareTo(x.LastAnswered);
//            });

//            //Calculate the start and end index for the given page
//            int start, end;
//            start = (page - 1) * perPage;
//            end = start + perPage;
//            end = end > answers.Count ? answers.Count : end;

//            //Return a list of questions with answers
//            var list = new List<QuestionAnswerModel>();

//            for (int i = start; i < end; i++)
//            {
//                list.Add(new QuestionAnswerModel
//                {
//                    Question = _questionMap[answers[i].QuestionId],
//                    Answer = new Answer
//                    {
//                        ChoiceIndex = answers[i].ChoiceBit,
//                        ChoiceAccept = answers[i].ChoiceAccept,
//                        ChoiceWeight = answers[i].ChoiceWeight,
//                        LastAnswered = answers[i].LastAnswered
//                    }
//                });
//            }

//            return list.ToPagedList(page, perPage);
//        }

//        /// <summary>
//        /// 
//        /// Gets the next 2 unanswered questions for given user. Questions are ordered by rank so
//        /// the next 2 questions will be the next 2 "best" questions.
//        /// 
//        /// </summary>
//        public IList<QuestionModel> Next2Questions(int profileId)
//        {
//            //Get users answers in a dictionary
//            var answers = _matchCalc.GetAnswerDict(profileId);
//            var list = new List<QuestionModel>(2);

//            int count = 0;

//            //Loop thru questions
//            foreach (var q in _questions)
//            {
//                if (answers.ContainsKey(q.Id))
//                {
//                    //user has answered question - skip
//                    continue;
//                }

//                //user hasn't answerd question, add to list
//                list.Add(q);

//                if (++count >= 2) break;      
//            }

//            return list;
//        }
//    }    
//}
