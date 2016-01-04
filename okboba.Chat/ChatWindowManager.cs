using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Chat
{
    public class ChatWindowInfo
    {
        public bool IsMinimized { get; set; }
        public int ProfileId { get; set; }
        public int ConversationId { get; set; }
        public string Nickname { get; set; }
    }

    public class ChatWindowManager
    {
        private Dictionary<int, List<ChatWindowInfo>> _chatWindows = new Dictionary<int, List<ChatWindowInfo>>();

        public void Add(int key, int profileId, string nickname)
        {
            List<ChatWindowInfo> list;

            if(!_chatWindows.TryGetValue(key, out list))
            {
                list = new List<ChatWindowInfo>();
                _chatWindows.Add(key, list);
            }

            list.Add(new ChatWindowInfo
            {
                ProfileId = profileId,
                IsMinimized = false,
                Nickname = nickname
            });
        }

        public void Remove(int key, int profileId)
        {
            List<ChatWindowInfo> list;

            if (_chatWindows.TryGetValue(key, out list))
            {
                list.RemoveAll(item => item.ProfileId == profileId);
                if (list.Count == 0) _chatWindows.Remove(key);
            }            
        }

        public IEnumerable<ChatWindowInfo> GetWindows(int key)
        {
            List<ChatWindowInfo> list;
            if (_chatWindows.TryGetValue(key, out list))
            {
                return list;
            }
            return Enumerable.Empty<ChatWindowInfo>(); //return empty list
        }
    }
}