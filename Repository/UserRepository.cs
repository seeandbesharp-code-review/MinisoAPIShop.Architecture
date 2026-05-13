using Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Repository
{
    public class UserRepository : IUserRepository
    {

        private readonly MyWebApiShopContext _context;
        public UserRepository(MyWebApiShopContext context)
        {
            _context = context;
        }

        async public Task<User> GetUserById(int id)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            return user;
        }

        async public Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        async public Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        async public Task<User> AddUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        async public Task<User> LoginUser(User loginUser)
        {
            User? user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginUser.Email && u.Password == loginUser.Password);
            return user;
        }

        async public Task UpdateUser(int id, User myUser)
        {
            User user1 = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user1 != null)
            {
                user1.FirstName = myUser.FirstName;
                user1.LastName = myUser.LastName;
                user1.Phone = myUser.Phone;
                user1.City = myUser.City;
                user1.Street = myUser.Street;

                await _context.SaveChangesAsync();
            }
        }


    }
}
