using System.ComponentModel.DataAnnotations;

namespace Refeitorio.Models
{
    public enum UserType
    {
        [Display(Name = "Administrador")]
        Admin = 0,

        [Display(Name = "Funcionário")]
        Staff = 1,

        [Display(Name = "Aluno")]
        Student = 3
    }
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserType Role { get; set; }
        public string Nif { get; set; }
        public decimal Saldo { get; set; } = 0;

        public bool IsApproved { get; set; } = false;
    }
}
