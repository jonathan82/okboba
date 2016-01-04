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
using System.Configuration;
using okboba.Resources;

namespace okboba.Chat
{
    public class ChatInfo
    {
        //public Conversation LastConversation { get; set; }
        //public Profile Profile { get; set; }
        public int? ConversationId { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsOnline { get; set; }
        public string Nickname { get; set; }
    }

    public class MessageStatus
    {
        public int ConversationId { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
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
        private static ChatWindowManager _chatWindows = new ChatWindowManager();

        private IProfileRepository _profileRepo;
        private IMessageRepository _msgRepo;

        public ChatHub()
        {
            _profileRepo = EntityProfileRepository.Instance;
            _msgRepo = EntityMessageRepository.Instance;
        }

        /////////////////////////// Private Methods ///////////////////////////////////
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

        private string AvatarUrl(Profile profile)
        {            
            var photo = profile.GetFirstHeadshot(true);
            var url = "";

            if (string.IsNullOrEmpty(photo))
            {
                //Use one of the default avatars
                url = "/Content/images/";
                url += profile.Gender == OkbConstants.MALE ? "no-avatar-male-chat.png" : "no-avatar-female-chat.png";
            }
            else
            {
                //use user profile photo
                var baseUrl = ConfigurationManager.AppSettings["StorageUrl"] + OkbConstants.PHOTO_CONTAINER + "/";
                url = baseUrl + profile.UserId + "/" + photo;
            }
            return url;
        }


        ////////////////////////// Public Methods ////////////////////////////////////////

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
            var profile = _profileRepo.GetProfile(otherId);
            info.AvatarUrl = AvatarUrl(profile);
            info.ConversationId = convId;
            info.IsOnline = IsOnline(otherId);
            info.Nickname = profile.Nickname;

            if (convId==null)
            {
                //look for last conversation
                var lastConv = _msgRepo.GetLastConversation(me, otherId);
                info.ConversationId = (lastConv == null ? null : (int?)lastConv.Id);
            }

            if (info.ConversationId == null) return info;            

            //existing conversation get first page of last messages
            info.Messages = _msgRepo.GetMessages((int)info.ConversationId, 1);

            return info;
        }

        /// <summary>
        /// 
        /// Sends a message to the given user by adding it to the database and delivering to the user
        /// if they're online. 
        /// 
        /// </summary>
        public async Task<MessageStatus> SendMessageAsync(int to, string message, int? convId)
        {
            var from = GetProfileId();

            //Add message to database first - get the conversation ID in case new conversation
            //should probably wrap in try/catch block 
            convId = await _msgRepo.AddMessageAsync(from, to, message, convId);

            //Loop thru all the open connections for the user and 
            foreach (var connId in _connections.GetConnections(to))
            {
                Clients.Client(connId).receiveMessage(from, convId, message);
            }
            
            var status = new MessageStatus
            {
                ConversationId = (int)convId
            };

            return status;
        }

        public bool IsOnline(int profileId)
        {
            var conn = _connections.GetConnections(profileId);
            return conn.Count() != 0;
        }

        /*****************************************************************

            Functions for when clients connect/disconnect

        ******************************************************************/
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

        /*****************************************************************

            Functions for managing chat window state

        ******************************************************************/
        public void AddWindow(int profileId, string nickname)
        {
            var me = GetProfileId();
            _chatWindows.Add(me, profileId, nickname);
        }

        public void RemoveWindow(int profileId)
        {
            var me = GetProfileId();
            _chatWindows.Remove(me, profileId);
        }

        public IEnumerable<ChatWindowInfo> GetWindows()
        {
            var me = GetProfileId();
            return _chatWindows.GetWindows(me);
        }

        public void SetMinimized(bool isMinimized, int profileId)
        {

        }
    }
}