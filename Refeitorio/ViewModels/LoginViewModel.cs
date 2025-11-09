using System.ComponentModel.DataAnnotations;

namespace Refeitorio.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O Email é obrigatório")]
        public string Email { get; set; }
        [Required(ErrorMessage = "A Palavra-Passe é obrigatória")]
        public string Password { get; set; }
    }
}
