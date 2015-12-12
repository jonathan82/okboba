using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace okboba.Repository.EntityRepository
{
    public class EntityMessageRepository : IMessageRepository
    {
        #region Singelton
        private static EntityMessageRepository instance;
        private EntityMessageRepository() { }
        public static EntityMessageRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityMessageRepository();
                }
                return instance;
            }
        }

        public async Task AddMessageAsync(int from, int to, string text, int? convId)
        {
            var db = new OkbDbContext();
            Message msg;

            if (convId==null)
            {
                //// New conversation
                var conv = new Conversation { Subject = "" };
                db.Conversations.Add(conv);

                msg = new Message
                {
                    Conversation = conv,
                    From = from,
                    MessageText = text,
                    Timestamp = DateTime.Now
                };
                db.Messages.Add(msg);

                //My copy of the conversation
                var mapMe = new ConversationMap
                {
                    Conversation = conv,
                    LastMessage = msg,
                    Other = to,
                    ProfileId = from
                };
                db.ConversationMap.Add(mapMe);

                //Other's copy of the conversation
                var mapOther = new ConversationMap
                {
                    Conversation = conv,
                    LastMessage = msg,
                    Other = from,
                    ProfileId = to
                };
                db.ConversationMap.Add(mapOther);

                await db.SaveChangesAsync();

                return;
            }

            //// Add to existing conversation
            msg = new Message
            {
                ConversationId = (int)convId,
                From = from,
                MessageText = text,
                Timestamp = DateTime.Now
            };
            db.Messages.Add(msg);

            //Update last message in conversation
            
        }

        public IEnumerable<Conversation> GetConversations(int id)
        {
            throw new NotImplementedException();
        }

        public Conversation GetLastConversation(int id, int other)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Message> GetMessages(int convId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Conversation> GetSent(int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        //public IEnumerable<Message> GetMessages(int convId, int page = 1, int numPerPage = 20)
        //{
        //    var db = new OkbDbContext();

        //    var result = from msg in db.Messages.AsNoTracking()
        //                 where msg.ConversationId == convId
        //                 orderby msg.Timestamp descending
        //                 select msg;

        //    var skip = (page - 1) * numPerPage;

        //    return result.Skip(skip).Take(numPerPage);
        //}

        //public Conversation GetLastConversation(int me, int them)
        //{
        //    var db = new OkbDbContext();

        //    var query = from cm in db.ConversationMap.AsNoTracking()
        //                where cm.ProfileId == me && 
        //                (cm.Conversation.PersonA == them || cm.Conversation.PersonB == them)
        //                orderby cm.Conversation.LastMessage.Timestamp descending
        //                select cm.Conversation;

        //    return query.First();
        //}

        //public void AddMessage(int from, int to, string text, int convId)
        //{
        //    var db = new OkbDbContext();

        //    //add the message
        //    var msg = new Message
        //    {
        //        From = from,
        //        MessageText = text,
        //        Timestamp = DateTime.Now,
        //        ConversationId = convId
        //    };

        //    //update the conversation with the latest message
        //    var conv = db.Conversations.Find(convId);
        //    conv.LastMessage = msg;
        //    db.SaveChanges();
        //}
    }
}
