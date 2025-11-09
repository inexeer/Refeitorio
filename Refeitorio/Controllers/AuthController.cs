using Microsoft.AspNetCore.Mvc;
using Refeitorio.Models;
using Refeitorio.ViewModels;
using Refeitorio.Services;

namespace Refeitorio.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserService m_userService;
        public AuthController(UserService userService)
        {
            m_userService = userService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (m_userService.Login(model.Email, model.Password))
                return RedirectToAction("Index", "Home");
            ViewBag.Error = "Email ou palavra-passe inválido!";
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(string email, string password, string fullname, UserType role)
        {
            if (m_userService.Register(email, password, fullname, role))
                return RedirectToAction("Login");
            ViewBag.Error = "Utilizador já existe!";
            return View();
        }
    }
}
