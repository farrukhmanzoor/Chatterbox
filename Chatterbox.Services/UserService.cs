using Chatterbox.Contracts;
using Chatterbox.Models;
using Microsoft.EntityFrameworkCore;

namespace Chatterbox.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable< User>> SearchUsers(string searchText = null)
        {
            if (searchText != null)
            {
                searchText = searchText.Trim().ToLower();
            }
            var usersList = await _context.Users.OrderBy(c => c.Username)
                .Select(u=> new User() { Id = u.Id, Username = u.Username })
                .Where(u => u.Username.ToLower().Contains(searchText))
                .ToListAsync();
            return usersList.ToList();
        }


        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
