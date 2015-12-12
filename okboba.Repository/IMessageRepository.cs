using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Requirements for Messaging/Chat system: 
/// 
/// 1. Get a list of messages in a conversation (Reply view)
///     - Avatar of other user
///     - Message text 
///     - Timestamp
/// 
/// 2. Get a list of conversations for a user (Inbox)
///     - Photo avatar
///     - Name
///     - Blurb of last message
///     - Date
///     - Has been read?
///     - Has at least one reply (not a message the user sent that has no response)
/// 
/// 3. Get a list of Sent messages for a user (Sent)
///     - Sent: a conversation where the last message sent was by that user
/// 
/// 4. Get the last conversation with a given user
///     - Used when Chat window is created
/// 
/// 5. Add message to a conversation (Send Message)
///     - Add message to Messages table
///     - Update the ConversationMap with the last message sent
///     - Update HasReply flag
///     - Add new Conversation/ConversationMap if needed (convId == null)
/// 
/// Notes: All the lists returned should be paged.
/// 
/// </summary> 

namespace okboba.Repository
{
    interface IMessageRepository
    {
        IEnumerable<Message> GetMessages(int convId);
        IEnumerable<Conversation> GetConversations(int id);
        IEnumerable<Conversation> GetSent(int id);
        Conversation GetLastConversation(int id, int other);
        Task AddMessageAsync(int from, int to, string text, int? convId);
    }
}
