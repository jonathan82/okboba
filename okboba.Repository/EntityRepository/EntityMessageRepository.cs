﻿using okboba.Entities;
using okboba.Repository.Models;
using okboba.Resources;
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
        #endregion

        /// <summary>
        /// 
        /// Adds a new message to the Messages table.  Creates a new converation if necessary.
        ///  - Updates the LastMessage to point to new message in the ConversationMap table. 
        ///  - Sets the HasBeenRead and HasReply flags
        /// 
        /// Returns the conversation Id the message was added to
        /// </summary>
        public async Task<int> AddMessageAsync(int from, int to, string text, int? convId = null)
        {
            var db = new OkbDbContext();
            Message msg;
            ConversationMap mapMe, mapOther;

            if (convId==null)
            {
                //// New conversation
                var conv = new Conversation { Subject = "Hi" };
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
                mapMe = new ConversationMap
                {
                    Conversation = conv,
                    LastMessage = msg,
                    Other = to,
                    ProfileId = from,
                    HasBeenRead = true,
                    HasReplies = false
                };
                db.ConversationMap.Add(mapMe);

                //Other's copy of the conversation
                mapOther = new ConversationMap
                {
                    Conversation = conv,
                    LastMessage = msg,
                    Other = from,
                    ProfileId = to,
                    HasBeenRead = false,
                    HasReplies = true
                };
                db.ConversationMap.Add(mapOther);

                await db.SaveChangesAsync();

                return conv.Id; //return new conversation Id
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
            mapMe = db.ConversationMap.Find(from, convId);
            mapMe.HasBeenRead = true;

            mapOther = db.ConversationMap.Find(to, convId);
            mapOther.HasBeenRead = false;

            if (!mapMe.HasReplies || !mapOther.HasReplies)
            {
                //Conversation doesn't have replies.  Check if this message is a reply 
                //and set flag accordingly.
                if(mapMe.LastMessage.From != from)
                {
                    //the last message isn't from me
                    mapMe.HasReplies = true;
                    mapOther.HasReplies = true;
                }
            }

            mapMe.LastMessage = msg;
            mapOther.LastMessage = msg;

            await db.SaveChangesAsync();

            return (int)convId; //return passed in conversation Id
        }


        /// <summary>
        /// 
        /// Gets a paged list of conversations belonging to the given user, 
        /// ordered by the last message sent. Shows only conversations that have at least
        /// one reply (not ones where only one user has sent a message).
        /// 
        /// </summary>
        public IEnumerable<ConversationModel> GetConversations(int id, int page = 1, int numPerPage = 20)
        {
            var db = new OkbDbContext();

            var query = from map in db.ConversationMap.AsNoTracking()
                        where map.ProfileId == id && map.HasReplies == true
                        orderby map.LastMessage.Timestamp descending
                        select new ConversationModel
                        {
                            OtherProfile = map.OtherProfile,
                            LastMessage = map.LastMessage,
                            HasBeenRead = map.HasBeenRead
                        };

            var skip = (page - 1) * numPerPage;

            return query.Skip(skip).Take(numPerPage);
        }


        /// <summary>
        /// 
        /// Gets the last conversation given user has had with other user.
        /// 
        /// </summary>
        public Conversation GetLastConversation(int me, int other)
        {
            var db = new OkbDbContext();

            var query = from map in db.ConversationMap.AsNoTracking()
                        where map.ProfileId == me && map.Other == other
                        orderby map.LastMessage.Timestamp descending
                        select map.Conversation;

            return query.FirstOrDefault();
        }


        /// <summary>
        /// Gets a list of messages for the given conversation Id ordered by most recent messages first.
        /// </summary>
        public IList<Message> GetMessages(int convId, int low = 0, int take = 5)
        {
            var db = new OkbDbContext();

            var query = from msg in db.Messages.AsNoTracking()
                        where msg.ConversationId == convId
                        orderby msg.Timestamp descending
                        select msg;

            return query.Skip(low).Take(take).ToList();
        }


        /// <summary>
        /// 
        /// Gets a paged list of Sent conversations for the given user.  A Sent conversation
        /// is one where the user sent the last message. Ordered by timestamp.
        /// 
        /// </summary>
        public IEnumerable<ConversationModel> GetSent(int id, int low, int numPerPage)
        {
            var db = new OkbDbContext();

            var query = from map in db.ConversationMap.AsNoTracking()
                        where map.ProfileId == id && map.LastMessage.From == id
                        orderby map.LastMessage.Timestamp descending
                        select new ConversationModel
                        {
                            OtherProfile = map.OtherProfile,
                            LastMessage = map.LastMessage,
                            HasBeenRead = map.HasBeenRead
                        };

            return query.Skip(low).Take(numPerPage);                        
        }


        /// <summary>
        /// 
        /// Get the total number of messages (sent + received) for a user
        /// 
        /// </summary>
        public int GetMessageCount(int id)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// Deletes the conversation for a user from ConversationMap.  Decrements the 
        /// message count for the user by the number of messages in conversation.
        /// 
        /// </summary>
        public async Task DeleteConversationAsync(int id, int convId)
        {
            var db = new OkbDbContext();

            //TODO: Get number of messages in conversation
            //var query = from msg in db.Messages.AsNoTracking()
            //            where msg.ConversationId == convId
            //            select msg;

            //var count = query.Count();

            //Delete conversation
            var toDelete = new ConversationMap { ConversationId = convId, ProfileId = id };
            db.ConversationMap.Attach(toDelete);
            db.ConversationMap.Remove(toDelete);

            //TODO: Decrement message count for user

            await db.SaveChangesAsync();
        }

        public ConversationMap GetConversationMap(int profileId, int convId)
        {
            var db = new OkbDbContext();
            var map = db.ConversationMap.Find(profileId, convId);
            return map;
        }

        public void MarkAsRead(int profileId, int convId)
        {
            var db = new OkbDbContext();
            var map = db.ConversationMap.Find(profileId, convId);
            if(map != null)
            {
                map.HasBeenRead = true;
                db.SaveChanges();
            }            
            //var map = new ConversationMap
            //{
            //    ProfileId = profileId,
            //    ConversationId = convId,
            //    HasBeenRead = true
            //};
            //db.ConversationMap.Attach(map);
            //db.Entry(map).State = System.Data.Entity.EntityState.Modified;            
        }
    }
}
