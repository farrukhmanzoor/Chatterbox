using Chatterbox.Contracts;
using Chatterbox.Models;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Security.Claims;

namespace Chatterbox.Web.HangFire
{
    public interface IHangFireService
    {
        void ScheduleJobs();
    }

    public class HangFireService : IHangFireService
    {
        private readonly IMessageService messageService;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly IUserService userService;
        public HangFireService(IMessageService messageS, IHubContext<ChatHub> hub, IUserService userS)
        {
            messageService = messageS;
            hubContext = hub;
            userService = userS;
        }

        public async Task ResendUndeliveredMessagesAsync()
        {
          var undeliveredMessages = await messageService.GetUndeliveredMessagesAsync();

            var user = new User();
            foreach (var message in undeliveredMessages)
            {

                if (ChatHub.activeUsers.ContainsKey(message.RecipientId))
                {
                    user = await userService.GetUserByIdAsync(message.UserId);
                    await hubContext.Clients.Client(ChatHub.activeUsers[message.RecipientId]).SendAsync("ReceiveMessage", message, user.Username);

                    // Mark the message as delivered
                    await messageService.UpdateMessageDelivery(message.UserId);
                }
            }
        }

        public void ScheduleJobs()
        {
            // Schedule a recurring job to resend undelivered messages
            
            

            RecurringJob.AddOrUpdate(
                "resend-undelivered-messages",
                () => this.ResendUndeliveredMessagesAsync(),
                Cron.Minutely);
        }

    }


    public class HangfireCustomActivator(IServiceScopeFactory container) : JobActivator
    {
        public override object ActivateJob(Type type)
        {
            using var scope = container.CreateScope();
            return scope.ServiceProvider.GetRequiredKeyedService<HangFireService>(type.Name);
        }
    }
}
