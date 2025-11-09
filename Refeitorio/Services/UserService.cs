using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using Refeitorio.Models;
using System.IO;

namespace Refeitorio.Services
{
    public class UserService
    {
        private const string FilePath = "users.json";
        private List<User> m_users = new();

        public UserService()
        {
            if (File.Exists(FilePath))
            {
                var json = File.Exists(FilePath) ? File.ReadAllText(FilePath) : "[]";
                m_users = string.IsNullOrWhiteSpace(json) ? new List<User>() : JsonSerializer.Deserialize<List<User>>(json)!;
            }
        }

        private void Save()
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(m_users, new JsonSerializerOptions { WriteIndented = true }));
        }

        public string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public bool Register(string email, string password, string fullname, UserType role)
        {
            if (m_users.Any(u => u.Email == email))
            {
                return false;
            }

            m_users.Add(new User
            {
                Email = email,
                PasswordHash = HashPassword(password),
                FullName = fullname,
                Role = role
            });

            Save();
            return true;
        }

        public bool Login(string email, string password)
        {
            var user = m_users.FirstOrDefault(u => u.Email == email);
            if (user == null) return false;

            return user.PasswordHash == HashPassword(password);
        }
    }
}
