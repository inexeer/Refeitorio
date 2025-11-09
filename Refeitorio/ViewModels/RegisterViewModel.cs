using Refeitorio.Models;
using System.ComponentModel.DataAnnotations;

namespace Refeitorio.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O nome completo é obrigatório")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "A palavra-passe é obrigatória")]
        [MinLength(8, ErrorMessage = "Min 8 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirme a password")]
        [Compare("Password", ErrorMessage = "As palavras-passe devem ser iguais")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Selecione um cargo")]
        public UserType Role { get; set; }

        [Required(ErrorMessage = "O NIF é obrigatório")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Formato inválido")]
        public string Nif { get; set; }
    }
}
