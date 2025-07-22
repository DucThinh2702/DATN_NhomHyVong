using DATN.Models;
using DATN.Service;

namespace DATN.IRepository
{
    public interface IUsersRepository 
    {
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> CreateUser(User user);
        Task<User> GetUserById(int id);
        Task<User> DetailUserById(int id);
        User GetUserBySearchFullname(string fullname);
        User GetUserByEmail(string email, string pass);
        Task UpdateUser(User user);
        Task DeleteUser(int id);
        Task<User> RegisterAsync(User user);
        bool EmailExists(string email);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsPhoneNumberExistsAsync(string phoneNumber);
        Task<bool> IsEmailExistsAsync(string email,int id);
        Task<bool> IsUsernameExistsAsync(string username, int id);
        Task<bool> IsPhoneNumberExistsAsync(string phoneNumber, int id);
        Task<int> CountUsersAsync();
        bool IsValidUser(string email, string password);
    }
}
