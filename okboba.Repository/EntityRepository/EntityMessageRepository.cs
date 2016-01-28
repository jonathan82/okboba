using okboba.Entities;
using okboba.Repository.Models;
using okboba.Repository.RedisRepository;
using okboba.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace okboba.Repository.EntityRepository
{
    public class EntityMessageRepository : IMessageRepository
    {
        private SXGenericRepository _redis;

        #region Singelton
        private static EntityMessageRepository instance;

        private EntityMessageRepository(SXGenericRepository redis)
        {
            _redis = redis;
        }

        public static EntityMessageRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityMessageRepository(SXGenericRepository.Instance);
                }
                return instance;
            }
        }
        #endregion

        /// <summary>
        /// Starts a new conversation with the given user by adding their message to the database.
        /// returns the Id of the newly created conversation. Increments the unread count of the 
        /// user the message was sent to.
        /// </summary>
        public async Task<int> StartConversation(int from, int to, string subject, string message)
        {
            var db = new OkbDbContext();
            Message msg;
            ConversationMap mapMe, mapOther;

            //// New conversation
            var conv = new Conversation { Subject = subject };
            db.Conversations.Add(conv);

            msg = new Message
            {
                Conversation = conv,
                From = from,
                MessageText = message,
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

            //Everything OK - we can increment the unread count
            IncrementUnreadCount(to);

            return conv.Id; //return new conversation Id
        }

        /// <summary>
        /// Reply to an existing conversation. We don't need the "to" field since we only
        /// store the "from" field for each message. Increments the unread count for the
        /// user sent to if the message was previously unread. 
        /// </summary>
        public async Task Reply(int from, int convId, string message)
        {
            var db = new OkbDbContext();
            Message msg;
            ConversationMap mapMe, mapOther;
            bool incUnreadFlag = false;

            //// Add to existing conversation
            msg = new Message
            {
                ConversationId = convId,
                From = from,
                MessageText = message,
                Timestamp = DateTime.Now
            };
            db.Messages.Add(msg);

            //Update last message in conversation
            mapMe = db.ConversationMap.Find(from, convId);
            mapMe.HasBeenRead = true;

            mapOther = db.ConversationMap.Find(mapMe.Other, convId);

            if (mapOther==null)
            {
                //conversation doesn't exist for other user - they deleted it
                mapOther = new ConversationMap
                {
                    ConversationId = convId,
                    ProfileId = mapMe.Other,
                    Other = from,
                    HasBeenRead = false,
                    HasReplies = true 
                };
                db.ConversationMap.Add(mapOther);
                incUnreadFlag = true;
            }

            //Increment unread count for "to" user if convesation was previously unread
            if (mapOther.HasBeenRead)
            {
                incUnreadFlag = true;
            }
                        
            mapOther.HasBeenRead = false;
            mapOther.HasBeenEmailed = false;

            if (!mapMe.HasReplies || !mapOther.HasReplies)
            {
                //Conversation doesn't have replies.  Check if this message is a reply 
                //and set flag accordingly.
                if (mapMe.LastMessage.From != from)
                {
                    //the last message isn't from me
                    mapMe.HasReplies = true;
                    mapOther.HasReplies = true;
                }
            }

            mapMe.LastMessage = msg;
            mapOther.LastMessage = msg;

            await db.SaveChangesAsync();

            //do this last in case we have any db errors
            if (incUnreadFlag)
            {
                IncrementUnreadCount(mapMe.Other);
            }
        }

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
                            Map = map,
                            Conversation = map.Conversation,
                            OtherProfile = map.OtherProfile,
                            LastMessage = map.LastMessage
                            //HasBeenRead = map.HasBeenRead
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
                            Map = map,
                            Conversation = map.Conversation,
                            OtherProfile = map.OtherProfile,
                            LastMessage = map.LastMessage
                            //HasBeenRead = map.HasBeenRead
                        };

            return query.Skip(low).Take(numPerPage);                        
        }


        /// <summary>
        /// Gets the number of unread messages for a user. Looks in the cache first and if not there
        /// get from database and save in cache, with an expiration time
        /// </summary>
        public int GetUnreadCount(int profileId)
        {
            var key = "unreadcount:" + profileId;

            var cache = _redis.GetDatabase();

            var val = cache.StringGet(key);

            if(!val.IsNull)
            {
                //cache hit
                return (int)val;
            }
            
            // cache miss
            var db = new OkbDbContext();

            var query = from cm in db.ConversationMap.AsNoTracking()
                        where cm.HasBeenRead == false && cm.ProfileId == profileId
                        select cm;

            int count = query.Count();

            cache.StringSet(key, count, new TimeSpan(0, OkbConstants.CACHE_DEFAULT_EXPIRATION, 0));

            return count;
        }

        /// <summary>
        /// Decrement the unread count in the cache for the given user,
        /// if it exists in the cache
        /// </summary>
        public void DecrementUnreadCount(int profileId)
        {
            var key = "unreadcount:" + profileId;
            var db = _redis.GetDatabase();
            if(db.KeyExists(key))
            {
                db.StringDecrement(key);
            }
        }

        /// <summary>
        /// Decrement the unread count in the cache for the given user,
        /// if it exists in the cache
        /// </summary>
        public void IncrementUnreadCount(int profileId)
        {
            var key = "unreadcount:" + profileId;
            var cache = _redis.GetDatabase();
            if (cache.KeyExists(key))
            {
                cache.StringIncrement(key);
            }
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

        /// <summary>
        /// Marks a conversation as read. If the message was unread previoulsy then we
        /// decrement the unread count in the cache
        /// </summary>
        public void MarkAsRead(int profileId, int convId)
        {
            var db = new OkbDbContext();

            var map = db.ConversationMap.Find(profileId, convId);

            //Error check - make sure we're marking an existing conversation
            if (map != null)
            {
                //Decrement count and mark only if message was previously unread
                if (!map.HasBeenRead)
                {
                    DecrementUnreadCount(profileId);
                    map.HasBeenRead = true;
                    db.SaveChanges();
                }                
            }
        }

        public Conversation GetConversation(int id)
        {
            var db = new OkbDbContext();
            var conv =  db.Conversations.Find(id);
            return conv;
        }

        /// <summary>
        /// Gets all the unread conversations in the database that haven't been sent to email.
        /// Used by the Mail Job to notify users of unread mail.
        /// </summary>
        public IEnumerable<UnreadConversationModel> GetUnreadConversations()
        {
            var db = new OkbDbContext();

            var result = from map in db.ConversationMap.AsNoTracking()
                         where map.HasBeenRead == false && map.HasBeenEmailed == false
                         join otherProfile in db.Profiles.AsNoTracking() on map.Other equals otherProfile.Id
                         join profile in db.Profiles.AsNoTracking() on map.ProfileId equals profile.Id
                         join user in db.Users.AsNoTracking() on profile.UserId equals user.Id
                         select new UnreadConversationModel
                         {
                             Map = map,
                             OtherProfile = otherProfile,
                             Email = user.Email,
                             UserId = user.Id,
                             Name = profile.Nickname
                         };

            return result.ToList();
        }

        /// <summary>
        /// Marks all the conversations as emailed. called by the mail job after it has finished 
        /// emailing all the users their unread conversations.
        /// </summary>
        public void MarkAllAsEmailed()
        {
            var db = new OkbDbContext();
            var sql = "update ConversationMaps set HasBeenEmailed = 1 where HasBeenEmailed = 0";
            db.Database.ExecuteSqlCommand(sql);            
        }
    }
}
