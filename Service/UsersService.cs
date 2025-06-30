using DATN.Data;
using DATN.IRepository;
using DATN.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DATN.Service
{
    public class UsersService(ApplicationDbContext context) : IUsersRepository
    {
        //private readonly List<User> _users = new List<User>();
        private readonly ApplicationDbContext _context = context;
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }
        public async Task<User> CreateUser(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id) ?? throw new KeyNotFoundException($"User with ID {id} not found.");
            return user;
        }
        public User GetUserByEmail(string email, string pass)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == pass)
                ?? throw new KeyNotFoundException($"User with email {email} not found.");
            return user;
        }
        public async Task UpdateUser(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var existingUser = _context.Users.Find(user.UserId) ?? throw new KeyNotFoundException($"User with ID {user.UserId} not found.");
            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            var user = _context.Users.Find(id) ?? throw new KeyNotFoundException($"User with ID {id} not found.");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
  

        public async Task<User> RegisterAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public bool EmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    }
}
