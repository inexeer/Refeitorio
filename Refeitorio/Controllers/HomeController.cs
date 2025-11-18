using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Refeitorio.Models;
using Refeitorio.Services;
using Refeitorio.ViewModels;

namespace Refeitorio.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService m_userService;
        private readonly AdminMenuService m_menuService;
        public HomeController(UserService userService, AdminMenuService menuService)
        {
            m_userService = userService;
            m_menuService = menuService;
        }

        public IActionResult AdminPendingUsers()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            var vm = new AdminPendingUsersViewModel
            {
                PendingUsers = m_userService.GetPendingUsers()
            };
            return PartialView("_AdminPendingUsers", vm);
        }

        public IActionResult AdminCreateMenus()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }
            return PartialView("_AdminCreateMenus");
        }

        public IActionResult AdminCreateLunch()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new ListMenus 
            {
                Menus = m_menuService.Menus
            };

            return PartialView("_AdminCreateLunch", model);
        }

        public IActionResult AdminLunchForm()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }
            return PartialView("_AdminLunchForm");
        }

        [HttpPost]
        public IActionResult Create(MenuDay model)
        {
            if (ModelState.IsValid){
                model.Id = m_menuService.GetAll().Any()
            ? m_menuService.GetAll().Max(m => m.Id) + 1
            : 1;
                m_menuService.Add(model);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public IActionResult DeleteLunch(int id)
        {
            var lunch = m_menuService.Menus.FirstOrDefault(x => x.Id == id);
            if (lunch != null)
            {
                m_menuService.Menus.Remove(lunch);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AdminSelectLunchByDate(string date)
        {
            var dt = DateOnly.Parse(date);
            var allLunches = m_menuService.Menus;
            var vm = new SelectLunchViewModel
            {
                Data = dt,
                VegOptions = allLunches
                    .Where(x => x.Option == MenuOption.Vegetariano)
                    .Select(x => new SelectListItem(
                        text: $"{x.MainDish} (Vegetariano)",
                        value: x.Id.ToString()
                    ))
                    .ToList(),

                NormalOptions = allLunches
                    .Where(x => x.Option == MenuOption.Normal)
                    .Select(x => new SelectListItem(
                        text: $"{x.MainDish} (Normal)",
                        value: x.Id.ToString()
                    ))
                    .ToList()
            };
            return PartialView("_AdminSelectLunchByDate", vm);
        }

        [HttpPost]
        public IActionResult AssociateMenu(SelectLunchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var chosenId = model.SelectedVegetarianId ?? model.SelectedNormalId ?? 0;
            var isVeg = model.SelectedVegetarianId.HasValue;
            m_menuService.LunchByDate[model.Data] = new MenuDay
            {
                Id = chosenId,
                Option = isVeg ? MenuOption.Vegetariano : MenuOption.Normal
            };

            return RedirectToAction("Index");
        }

        //public IActionResult Create(MenuDay model)
        //{
        //    var role = HttpContext.Session.GetString("Role");
        //    if (role != "Admin")
        //    {
        //        return RedirectToAction("Login", "Auth");
        //    }
        //}

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || (role != "Admin" && role != "Staff" && role != "Student"))
            {
                return RedirectToAction("Login", "Auth");
            }
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

            //if (role == "Admin")
            //{
            //    var vm = new AdminPendingUsersViewModel
            //    {
            //        PendingUsers = m_userService.GetPendingUsers()
            //    };
            //    return View(vm);
            //}

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