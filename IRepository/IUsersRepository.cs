using DATN.Models;
using DATN.Service;

namespace DATN.IRepository
{
    public interface IUsersRepository 
    {
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> CreateUser(User user);
        Task<User> GetUserById(int id);
        User GetUserByEmail(string email, string pass);
        Task UpdateUser(User user);
        Task DeleteUser(int id);
        Task<User> RegisterAsync(User user);
        bool EmailExists(string email);
    }
}
