using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Refeitorio.Models;
using Refeitorio.Services;
using Refeitorio.ViewModels;

namespace Refeitorio.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService m_userService;

        public HomeController(UserService userService)
        {
            m_userService = userService;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            // obter o nome do utilizador (se estiver logado)
            var userEmail = HttpContext.Session.GetString("User");
            if (!string.IsNullOrEmpty(userEmail))
            {
                var user = m_user_service_get(userEmail);
                if (user != null)
                {
                    ViewBag.UserName = user.FullName;
                    // calculo de iniciais (caso queiras mostrar as iniciais em vez dum avatar)
                    var names = user.FullName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                    var initials = "";
                    if (names.Length > 0) initials += names[0][0];
                    if (names.Length > 1) initials += names[^1][0];
                    ViewBag.UserInitials = initials.ToUpper();
                }
            }

            if (role == "Admin")
            {
                var vm = new AdminPendingUsersViewModel
                {
                    PendingUsers = m_userService.GetPendingUsers()
                };
                return View(vm);
            }

            return View(new AdminPendingUsersViewModel());
        }

        [HttpPost]
        public IActionResult ApproveUser(Guid id)
        {
            m_userService.ApproveUser(id);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // helper to avoid nullable reference warnings when looking up user by email (keeps logic clearer)
        private Refeitorio.Models.User? m_user_service_get(string email)
        {
            return m_userService.GetUserByEmail(email);
        }
    }
}