using DATN.Data;
using DATN.IRepository;
using DATN.Models;
using Microsoft.AspNetCore.Mvc;
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

            // Kiểm tra trùng Email, Username, PhoneNumber
            var existingUser = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == user.Email ||
                u.Username == user.Username ||
                u.PhoneNumber == user.PhoneNumber);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Email, Username hoặc Số điện thoại đã tồn tại.");
            }

            // Bắt buộc phải có Password
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new InvalidOperationException("Mật khẩu không được để trống.");
            }

            // Hash mật khẩu
            user.Password = HmacSHA256(user.Password??"");
          
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> RegisterAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User không được null.");
            }
            if (user.Email != null && EmailExists(email: user.Email))
            {
                throw new InvalidOperationException("Email đã được sử dụng.");
            }
            // Hash mật khẩu
            user.Password = HmacSHA256(user.Password??"");

            // Gán ngày tạo nếu có thuộc tính
            user.CreatedDate = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id) ?? throw new KeyNotFoundException($"User with ID {id} not found.");
            return user;
        }
        public async Task<User> DetailUserById(int id)
        {
            var user = await _context.Users.FindAsync(id) ?? throw new KeyNotFoundException($"User with ID {id} not found.");
            return user;
        }
        public User GetUserBySearchFullname(string fullname)
        {
            var user = _context.Users.Find(fullname)
                ?? throw new KeyNotFoundException($"User with fullname {fullname} not found.");
            return user;
        }
        public User GetUserByEmail(string email, string pass)
        {
            string hashedPassword = HmacSHA256(pass);

            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == hashedPassword)
                ?? throw new KeyNotFoundException($"Email hoặc mật khẩu không đúng.");

            return user;
        }

        private static string HmacSHA256(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hash = System.Security.Cryptography.SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        public async Task UpdateUser(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var existingUser = await _context.Users.FindAsync(user.UserId)
                ?? throw new KeyNotFoundException($"User với ID {user.UserId} không tồn tại.");

            // Nếu có mật khẩu mới → hash lại, nếu không thì giữ mật khẩu cũ
            var passwordToUse = string.IsNullOrWhiteSpace(user.Password)
                ? existingUser.Password
                : HmacSHA256(user.Password);

            var updatedUser = new User
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                Gender = user.Gender,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Status = user.Status,
                RoleId = user.RoleId,
                Password = passwordToUse, // có thể giữ hoặc cập nhật
                CreatedDate = existingUser.CreatedDate
            };
            _context.Entry(existingUser).CurrentValues.SetValues(updatedUser);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            var user = _context.Users.Find(id) ??
                throw new KeyNotFoundException($"User with ID {id} not found.");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }      
        public bool EmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    }
}
