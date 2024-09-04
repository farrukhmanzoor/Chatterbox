
using Chatterbox.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Chatterbox.Models;

namespace Chatterbox.Web
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUserService authenticatedUserService;
        private readonly IMessageService privateMessageService;
        public static readonly Dictionary<int, string> activeUsers = new();

        public ChatHub(
            IUserService authenticatedUserService,
            IMessageService privateMessageService)
        {
            this.authenticatedUserService = authenticatedUserService;
            this.privateMessageService = privateMessageService;
        }
        //broadcast message to all users in a group.
        public async Task SendMessageToAll( int userId, string message)
        {
            
            await Clients.Others.SendAsync("ReceiveMessage", userId, message);
        }

        public async Task SendMessageToUser(Message message)
        {
            int userId = message.RecipientId;
            string username = GetAuthenticatedUsername();
            await privateMessageService.AddMessageAsync(message);
            if (activeUsers.ContainsKey(userId))
            {
                await Clients.Client(activeUsers[userId]).SendAsync("ReceiveMessage", message, username);
            }
        }

        public async Task AcknowledgeMessageReceived(Message message)
        {
            // Update the message status in the database to 'Received'
           await privateMessageService.UpdateMessageDelivery(message.Id);

            if (activeUsers.ContainsKey( message.UserId))
            {
                // Broadcast to the sender that the message was received
                await Clients.Caller.SendAsync("MessageAcknowledged", message);
            }
        }

        private string GetAuthenticatedUsername()
        {
            return this.Context.User.FindFirst(ClaimTypes.Name).Value;
        }

        public async Task AddUser(int userId, string connectionId)
        {
            activeUsers.Add(userId, connectionId);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public List<int> GetActiveUserIds()
        {
            return activeUsers.Keys.ToList();
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = GetConnectionId();
            var userId = GetAuthenticatedUserId();
            if (!activeUsers.ContainsKey(userId))
            {
                activeUsers.Add(userId, connectionId);
            }
            await Clients.All.SendAsync("ReceiveActiveUsers", GetActiveUserIds());
            await base.OnConnectedAsync();
        }

        private int GetAuthenticatedUserId()
        {
            string nameIdentifier = this.Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            return int.Parse(nameIdentifier);
            
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = GetConnectionId();

            foreach (var user in activeUsers)
            {
                if (user.Value == connectionId)
                {
                    activeUsers.Remove(user.Key);
                    break;
                }
            }
            await Clients.All.SendAsync("ReceiveActiveUsers", GetActiveUserIds());
            await base.OnDisconnectedAsync(exception);
        }
    }
}
