namespace Chatterbox.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
       // public ICollection<Message> Messages { get; set; }
    }

    public class Message
    {
        public Message()
        {
            
        }
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsDelivered { get; set; }
        public int RecipientId { get; set; }
        public bool IsRead { get; set; }
    }

    public class ChatWithLastMessage
    {
        public User User { get; set; }
        public Message LastMessage { get; set; }
        public int UnReadCount {get;set;}
    }

}
