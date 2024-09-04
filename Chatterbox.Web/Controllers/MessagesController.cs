using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Chatterbox.Contracts;
using Chatterbox.Models;
using Microsoft.AspNet.SignalR;
using Chatterbox.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Chatterbox.Web.DTO;


namespace Chatterbox.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService messageService;
        public MessagesController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet()]
        [Route("getMessages")]
        public async Task<IActionResult> GetPrivateMessages(
            DateTime? pageDate,
            int pageSize,
            int firstUserId,
            int secoundUserId)
        {
            var result = await messageService.GetPrivateMessagesForUserAsync(pageDate, pageSize, firstUserId, secoundUserId);
            return Ok(new {messages =result, isThereMore = false });
        }

        [HttpGet()]
        [Route("getRecentChats")]
        public async Task<IActionResult> GetRecentChatsForUser([FromQuery] int userId)
        {

            var result = await messageService.GetRecentChatsForUser(userId);
            return Ok(result);

        }


        [HttpPost()]
        [Route("markAsRead")]
        public async Task<IActionResult> MarkMessagesAsRead([FromBody] MarkMessagesAsRead request)
        {
            // Ensure that the request is valid
            if (request.UserId == 0 || request.RecipientId == 0)
            {
                return BadRequest("Invalid user or chat user ID.");
            }
            bool result = await messageService.MarkMessagesAsReadAsync(request.UserId, request.RecipientId);
          

            return Ok(result);
        }

    }
}
