using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace okboba.Instant
{
    public class ConnectionMapping
    {
        private readonly Dictionary<int, HashSet<string>> _connections = new Dictionary<int, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public IEnumerable<string> GetConnections(int key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Add(int profileId, string connectionId)
        {
            HashSet<string> connections;
            
            if(!_connections.TryGetValue(profileId, out connections))
            {
                connections = new HashSet<string>();
                _connections.Add(profileId, connections);
            }

            connections.Add(connectionId);
        }

        public void Remove(int key, string connectionId)
        {
            HashSet<string> connections;

            if(!_connections.TryGetValue(key, out connections))
            {
                return;
            }

            connections.Remove(connectionId);

            if (connections.Count==0)
            {
                _connections.Remove(key);
            }
        }


    }
}