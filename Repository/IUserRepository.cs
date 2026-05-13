//using Entities;
using Entities;

namespace Repository
{
    public interface IUserRepository
    {
        Task<User> AddUser(User user);
        Task<User> GetUserById(int id);
        Task<bool> IsEmailExistsAsync(string email);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> LoginUser(User loginUser);
        Task UpdateUser(int id, User myUser);
    }
}