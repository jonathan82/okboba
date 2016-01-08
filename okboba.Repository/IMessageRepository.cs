using okboba.Entities;
using okboba.Repository.Models;
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
///     - Set HasBeenRead = false
///     - Add new Conversation/ConversationMap if needed (convId == null)
///     - Increment the message count for both users (from/to)
/// 
/// 6. Keep track of the total message count for the user (sent + received)
///     - Get message count 
/// 
/// 7. Delete a Conversation
///     - Decrement the message count by # of messages in conversation
///     - Remove from ConversationMap.  Doesn't affect other's users copy of the conversation.
/// 
/// Notes: All the lists returned should be paged.
/// 
/// </summary> 

namespace okboba.Repository
{
    public interface IMessageRepository
    {
        IList<Message> GetMessages(int convId, int low = 0, int take = 5);
        IEnumerable<ConversationModel> GetConversations(int id, int page = 1, int numPerPage = 20);
        IEnumerable<ConversationModel> GetSent(int id, int low = 0, int numPerPage = 20);
        Conversation GetLastConversation(int id, int other);
        Task<int> AddMessageAsync(int from, int to, string text, int? convId);
        int GetUnreadCount(int profileId);
        void DecrementUnreadCount(int profileId);
        Task DeleteConversationAsync(int id, int convId);
        ConversationMap GetConversationMap(int profileId, int convId);
        void MarkAsRead(int profileId, int convId);
    }
}
