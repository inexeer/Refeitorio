using System.ComponentModel.DataAnnotations;

namespace Refeitorio.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Insira o Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Insira a Palavra-Passe")]
        public string Password { get; set; }
    }
}
