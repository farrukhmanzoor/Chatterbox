using Chatterbox.Contracts;
using Chatterbox.Models;
using Microsoft.EntityFrameworkCore;

namespace Chatterbox.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;

        public MessageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetPrivateMessagesForUserAsync(
          DateTime? pageDate,
          int pageSize,
          int currentUserId,
          int recipientId)
        {
            var result = new List<Message>();

            if (pageDate == null) pageDate = DateTime.Now;



            var messages = _context.Messages
               .Where(m => (m.UserId == currentUserId && m.RecipientId == recipientId) ||
                          (m.UserId == recipientId && m.RecipientId == currentUserId))
               .Where(m => m.Timestamp < pageDate)
               .AsQueryable();
            var messagesCount = await messages.CountAsync();
            var isThereMore = messagesCount > pageSize;
            var messagesList = await messages
                .OrderByDescending(c => c.Timestamp)
                .Take(pageSize)
                .OrderBy(c => c.Timestamp)
                .ToListAsync();
            result = messagesList.ToList();


            return result;
        }


        public async Task<IEnumerable<ChatWithLastMessage>> GetRecentChatsForUser(int userId)
        {
            var recentChatsWithLastMessages = await _context.Messages
                .Where(m => m.UserId == userId || m.RecipientId == userId)
                .GroupBy(m => m.UserId == userId ? m.RecipientId : m.UserId)
                .OrderByDescending(g => g.Max(m => m.Timestamp))
                .Take(10)
                .Select(g => new ChatWithLastMessage
                {
                    User = _context.Users.Where(u => u.Id == g.Key).First(),
                    LastMessage = g.OrderByDescending(msg => msg.Timestamp).First(),
                    UnReadCount = g.Count(m => m.RecipientId == userId && !m.IsRead)
                })
                .ToListAsync();
            return recentChatsWithLastMessages;
        }

        public async Task AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

        }
        //Mark unread messages as read for a user.
        public async Task<bool> MarkMessagesAsReadAsync(int userId, int recipientId)
        {
            // Logic to mark messages as read
            var messages = await _context.Messages
                .Where(m => (m.UserId == userId && m.RecipientId == recipientId) ||
                            (m.UserId == recipientId && m.RecipientId == userId)   
                            && !m.IsRead)
                .ToListAsync();

            messages.ForEach(m => m.IsRead = true);

            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<Message>> GetUndeliveredMessagesAsync()
        {
            //throw new MissingMethodException();
            return await _context.Messages
                .Where(m=> !m.IsDelivered)
                .ToListAsync();
        }

        // Update the message status in the database to 'Delivered'
        //If last message is delivered assume all previous messages are delivered.
        public async Task<bool> UpdateMessageDelivery(int messageId)
        {
            int result = 0;
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == messageId);
            if (message != null)
            {
                var olderMessages = await _context.Messages.Where(m => m.Timestamp <= message.Timestamp).ToListAsync();
                olderMessages.ForEach(m => m.IsDelivered = true);

                 result = await _context.SaveChangesAsync();
                
            }
            return result > 0;
        }

    }
}
