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
    }
}
