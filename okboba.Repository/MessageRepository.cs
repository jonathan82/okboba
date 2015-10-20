using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using okboba.Entities;
using PagedList;
using System.Collections;

namespace okboba.Repository
{
    public class MessageRepository
    {
        #region Singelton
        private static MessageRepository instance;
        private MessageRepository() { }

        public static MessageRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MessageRepository();
                }
                return instance;
            }
        }
        #endregion

        #region Public Methods

        public class ConversationView
        {
            public Conversation Conversation { get; set; }
            public ConversationMap Map { get; set; }
        }

        public IQueryable<ConversationView> GetConversationList(int profileId )
        {
            var db = new OkbDbContext();

            var result = from cm in db.ConversationMap
                         join c in db.Conversations on cm.ConversationId equals c.Id
                         where cm.ProfileId == profileId orderby c.LastMessageDate descending
                         select new ConversationView {
                             Conversation = c,
                             Map = cm
                             //HasRead = cm.HasRead,
                             //WithProfileId = c.UserId1 == profileId ? c.UserId2 : c.UserId1,
                             //Photo = c.UserId1 == profileId ? c.UserPhoto2 : c.UserPhoto1,
                             //LastMessageDate = c.LastMessageDate,
                             //Blurb = c.LastMessageBlurb,
                             //Subject = c.Subject
                         };

            return result;
        }

        #endregion
    }
}
