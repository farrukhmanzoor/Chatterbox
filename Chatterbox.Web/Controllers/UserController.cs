using Chatterbox.Contracts;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chatterbox.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
                this.userService = userService;
        }

        [HttpGet()]
        [Route("getUserById")]
        public async Task<IActionResult> GetUserById([FromQuery] int userId)
        {
            var user = await userService.GetUserByIdAsync(userId);
            return Ok(user);
        }

        [HttpGet()]
        [Route("searchUsers")]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
        {
            var user = await userService.SearchUsers(searchTerm);
            return Ok(user);
        }
    }
}
