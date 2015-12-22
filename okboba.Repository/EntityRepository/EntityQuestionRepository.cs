using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using okboba.Repository.Models;
using okboba.Repository.MemoryRepository;
using PagedList;

namespace okboba.Repository.EntityRepository
{
    public class EntityQuestionRepository : QuestionRepositoryBase, IQuestionRepository
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
        /// Gets a list of all the answer choices for a given question
        /// </summary>
        private IList<string> GetChoices(int id)
        {
            var db = new OkbDbContext();
            var list = new List<string>();
            var result = from choice in db.QuestionChoices.AsNoTracking()
                         where choice.QuestionId == id
                         orderby choice.Index ascending
                         select choice;

            foreach (var choice in result)
            {
                list.Add(choice.Text);
            }

            return list;
        }

        /// <summary>
        /// 
        /// Gets the next 2 un-answered questions for the user.
        /// 
        /// </summary>
        public IList<QuestionModel> Next2Questions(int profileId)
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

            var list = new List<QuestionModel>();

            int count = 0;

            //Loop thru all the questions
            foreach (var question in db.Questions.AsNoTracking())
            {
                if (set.Contains(question.Id)) continue;
                list.Add(new QuestionModel
                {
                    Id = question.Id,
                    Text = question.Text,
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
        public IPagedList<QuestionAnswerModel> GetQuestions(int profileId, int page, int perPage)
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

            var list = new List<QuestionAnswerModel>();

            int currQuesId = -1;

            foreach (var row in result)
            {
                if (row.Question.Id != currQuesId)
                {
                    //add new question
                    list.Add(new QuestionAnswerModel
                    {
                        Question = new QuestionModel
                        {
                            Id = row.Question.Id,
                            Text = row.Question.Text,
                            Choices = new List<string>()
                        },
                        Answer = row.Answer
                    });
                }

                //add the choices
                list[list.Count - 1].Question.Choices.Add(row.Choice.Text);

                currQuesId = row.Question.Id;
            }

            return list.ToPagedList(page, perPage);
        }

        public void Answer(Answer ans)
        {
            ans = ValidateAnswer(ans);
            AnswerQuestionDb(ans);
            
            //call match api to update answer cache

        }
    }
}
