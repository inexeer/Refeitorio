namespace Refeitorio.Models
{
    public enum UserType
    {
        Admin,
        Staff,
        Student
    }
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserType Role { get; set; }
    }
}
