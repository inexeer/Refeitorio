using Microsoft.AspNetCore.Mvc;
using Refeitorio.Models;
using Refeitorio.ViewModels;
using Refeitorio.Services;
using Microsoft.AspNetCore.Authorization;

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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = m_userService.GetUserByEmail(model.Email);

            if (user == null)
            {
                ViewBag.Error = "Email ou palavra-passe inválido!";
                return View(model);
            }

            if (!user.IsApproved)
            {
                ViewBag.Error = "A sua conta ainda não foi aprovada";
                return View(model);
            }

            if (m_userService.Login(model.Email, model.Password))
            {
                HttpContext.Session.SetString("User", model.Email);
                HttpContext.Session.SetString("Role", user.Role.ToString());
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Email ou palavra-passe inválido!";
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (m_userService.Register(model.Email, model.Password, model.FullName, model.Role, model.Nif))
                return RedirectToAction("Login");
            ViewBag.Error = "Utilizador já existe!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
