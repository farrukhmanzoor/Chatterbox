using Chatterbox.Contracts;
using Chatterbox.Models;
using Chatterbox.Web.Helper;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chatterbox.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userRepository;

        public AuthController(IUserService userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash)
            };
            await _userRepository.AddUserAsync(user);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(User userDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(userDto.Username);
            if (user != null && BCrypt.Net.BCrypt.Verify(userDto.PasswordHash, user.PasswordHash))
            {
                var token = new TokenGenerator().Generate(user);
                return Ok(new { Token = token,User = user });
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("getCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdentity = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetUserByIdAsync(int.Parse(userIdentity));
            return Ok(user);
        }
    }

}
