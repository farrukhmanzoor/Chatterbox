using Chatterbox.Models;

namespace Chatterbox.Contracts
{
    public interface IUserService
    {
       public Task<User> GetUserByIdAsync(int id);
       public Task<User> GetUserByUsernameAsync(string username);
       public Task AddUserAsync(User user);
       public Task<IEnumerable<User>> SearchUsers(string searchText = null);
    }

    

}
