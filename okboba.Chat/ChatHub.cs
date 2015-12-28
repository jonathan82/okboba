using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using okboba.Entities;
using Microsoft.AspNet.Identity;
using okboba.Repository;
using okboba.Repository.EntityRepository;

namespace okboba.Chat
{
    public class ChatInfo
    {
        public Conversation LastConversation { get; set; }
        public Profile Profile { get; set; }
        public IEnumerable<Message> Messages { get; set; }
    }

    /// <summary>
    /// Chat should support the following features:
    /// 
    ///  1. Keep track of online users.  Get a list of the online users
    ///  2. Keep track of the open chat windows for each user.  Get a list of open chat windows per user
    ///      - If a user navigates to another page their chat windows should "follow" them, as well as the state
    ///        of the window (open/minimized)
    ///  3. Send messages. Persist message to DB as well a deliver to online users
    ///      - If user is offline return status message saying user is offline and message delivered to inbox.
    ///      - SendMessage should return a status indicating what happened to the message
    ///  4. Get a list of initial messages for a conversation, as well as profile info like name, avatar, gender, etc
    ///     to load when the chat window is first opened.
    /// 
    /// </summary>
    [Authorize]
    public class ChatHub : Hub
    {        
        private static ConnectionMapping _connections = new ConnectionMapping();
        private IProfileRepository _profileRepo;
        private IMessageRepository _msgRepo;

        public ChatHub()
        {
            _profileRepo = EntityProfileRepository.Instance;
            _msgRepo = EntityMessageRepository.Instance;
        }

        protected int GetProfileId()
        {
            int profileId;
           
            var db = new OkbDbContext();
            var id = Context.User.Identity.GetUserId();
            var user = db.Users.Find(id);

            if (user == null)
            {
                // No profile exists for user!??!?
                throw new Exception("No profile exists for user");
            }

            profileId = user.Profile.Id;

            return profileId;
        }

        /// <summary>
        /// Gets the info for the chat window when it is first opened. If the convId passed in is null
        /// it will look for the last conversation with that user, otherwise it retreive messages for given
        /// conversation.  
        /// 
        /// When message is received by chat window it will already have a conversation Id. When a chat is initiated
        /// (send message) convId would be null.
        /// 
        ///  - Profile info (avatar, etc)
        ///  - First page of messages for the last conversation 
        /// 
        /// </summary>        
        public ChatInfo GetInitialInfo(int otherId, int? convId = null)
        {
            var me = GetProfileId();            

            var info = new ChatInfo();

            info.Profile = _profileRepo.GetProfile(otherId);

            if (convId==null)
            {
                //look for last conversation
                var lastConv = _msgRepo.GetLastConversation(me, otherId);
                convId = (lastConv == null ? null : (int?)lastConv.Id);
            }
            
            if (convId==null)
            {
                //new conversation
                return info;               
            }

            //existing conversation get first page of last messages
            info.Messages = _msgRepo.GetMessages((int)convId, 1);

            return info;
        }

        public void SendMessage(int who, string message)
        {
            var from = GetProfileId();

            foreach (var connId in _connections.GetConnections(who))
            {
                Clients.Client(connId).receiveMessage(from, message);
            }
        }

        public override Task OnConnected()
        {
            var id = GetProfileId();

            _connections.Add(id, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var id = GetProfileId();

            _connections.Remove(id, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var id = GetProfileId();

            if (!_connections.GetConnections(id).Contains(Context.ConnectionId))
            {
                _connections.Add(id, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
    }
}