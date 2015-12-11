using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using okboba.Entities;
using Microsoft.AspNet.Identity;

namespace okboba.Chat
{
    [Authorize]
    public class ChatHub : Hub
    {        
        private static ConnectionMapping _connections = new ConnectionMapping();

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