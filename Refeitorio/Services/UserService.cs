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

        public void AddSaldo(string email, decimal amount)
        {
            var index = m_users.IndexOf(m_users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));

            if (index != -1)
            {
                m_users[index].Saldo += amount;
                Save();
            }
        }

        public string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public bool Register(string email, string password, string fullname, UserType role, string nif)
        {
            if (m_users.Any(u => u.Email == email || u.Nif == nif))
            {
                return false;
            }

            var user = new User
            {
                Email = email,
                PasswordHash = HashPassword(password),
                FullName = fullname,
                Role = role,
                Nif = nif,
                IsApproved = role == UserType.Admin
            };

            m_users.Add(user);
            Save();
            return true;
        }

        public User? GetUserByEmail(string email)
        {
            return m_users.FirstOrDefault(u => u.Email == email);
        }

        public bool Login(string email, string password)
        {
            var user = GetUserByEmail(email);
            if (user == null) return false;

            if (!user.IsApproved)
            {
                return false;
            }

            return user.PasswordHash == HashPassword(password);
        }

        public List<User> GetPendingUsers()
        {
            return m_users.Where(u => !u.IsApproved).ToList();
        }

        public void ApproveUser(Guid id)
        {
            var user = m_users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.IsApproved = true;
                Save();
            }
        }

        public void RejectUser(Guid id)
        {
            var user = m_users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                m_users.Remove(user);
                Save();
            }
        }
    }
}
