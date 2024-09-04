using Chatterbox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatterbox.Contracts
{
    public interface IMessageService
    {

        public Task<IEnumerable<Message>> GetPrivateMessagesForUserAsync(DateTime? pageDate, int pageSize, int currentUserId, int recipientId);
        public Task AddMessageAsync(Message message);
        public Task<IEnumerable<ChatWithLastMessage>> GetRecentChatsForUser(int userId);
        public Task<IEnumerable<Message>> GetUndeliveredMessagesAsync();
        public Task<bool> MarkMessagesAsReadAsync(int userId, int recipientId);
        public Task<bool> UpdateMessageDelivery(int messageId);

    }
}
