using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Refeitorio.Models;
using Refeitorio.Services;
using Refeitorio.ViewModels;
using System.Text.Json;
using System.IO;
using System.Reflection;
using System;

namespace Refeitorio.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService m_userService;
        private readonly AdminMenuService m_menuService;
        private readonly FuncProductService m_productService;
        private readonly BookingService m_bookingService;
        private readonly OrderService m_orderService;
        public HomeController(UserService userService, AdminMenuService menuService, FuncProductService productService, BookingService bookingService, OrderService orderService)
        {
            m_userService = userService;
            m_menuService = menuService;
            m_productService = productService;
            m_bookingService = bookingService;
            m_orderService = orderService;
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
                m_menuService.SaveMenus();
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
            var chosenMenu = m_menuService.GetAll().FirstOrDefault(m => m.Id == chosenId);
            if (chosenMenu == null)
            {
                TempData["Error"] = "Menu nao encontrado.";
                return RedirectToAction("Index");
            }

            m_menuService.SaveLunch(chosenMenu, model.Data);

            m_menuService.SerializarDict();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetSavedLunches()
        {
            var data = m_menuService.LunchByDate
                .ToDictionary(
                    k => k.Key.ToString("yyyy-MM-dd"),
                    v => new
                    {
                        mainDish = v.Value.MainDish ?? "",
                        soup = v.Value.Soup ?? "",
                        dessert = v.Value.Dessert ?? "",
                        option = v.Value.Option.ToString()
                    });

            return Content(JsonSerializer.Serialize(data), "application/json");
        }

        [HttpGet]
        public IActionResult FuncCreateProduct()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Staff")
            {
                return RedirectToAction("Login", "Auth");
            }
            return PartialView("_FuncCreateProduct");
        }

        [HttpPost]
        public IActionResult CreateProduct(Product model)
        {
            if (ModelState.IsValid)
            {
                model.Id = m_productService.GetAll().Any()
            ? m_productService.GetAll().Max(m => m.Id) + 1
            : 1;
                m_productService.AddProduct(model);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = m_productService.GetAll().FirstOrDefault(x => x.Id == id);
            if (product != null)
            {
                m_productService.DeleteProduct(product);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult FuncEditProduct(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Staff")
                return RedirectToAction("Login", "Auth");

            var product = m_productService.GetById(id);
            if (product == null) return NotFound();

            return PartialView("_FuncEditProduct", product);
        }

        [HttpPost]
        public IActionResult EditProduct(Product updatedProduct)
        {
            if (ModelState.IsValid)
            {
                m_productService.UpdateProduct(updatedProduct);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public IActionResult StudentMenuCalendar()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Student")
                return RedirectToAction("Login", "Auth");

            return PartialView("_StudentMenuCalendar");
        }

        [HttpPost]
        public IActionResult BookLunch(string date)
        {
            var email = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(email))
                return Json(new { success = false, message = "Nao autenticado" });

            var menu = m_menuService.LunchByDate
                .FirstOrDefault(x => x.Key.ToString("yyyy-MM-dd") == date).Value;

            if (menu == null)
                return Json(new { success = false, message = "Sem menu nesse dia" });

            m_bookingService.BookLunch(email, date, menu.Option.ToString());

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetMyBookings()
        {
            var email = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(email)) return Json(new { });

            var bookings = m_bookingService.Bookings
                .Where(b => b.UserEmail == email)
                .ToDictionary(b => b.Date, b => b.Option);

            return Json(bookings);
        }

        public IActionResult StudentBookingHistory()
        {
            var role = HttpContext.Session.GetString("Role");
            var userEmail = HttpContext.Session.GetString("User");
            if (role != "Student")
            {
                return RedirectToAction("Login", "Auth");
            }

            var vm = new StudentHistoryViewModel
            {
                Bookings = m_bookingService.GetByUser(userEmail)
            };
            return PartialView("_StudentHistorico", vm);
        }

        [HttpPost]
        public IActionResult CancelBooking(int id)
        {
            var booking = m_bookingService.Bookings.FirstOrDefault(x => x.Id == id);
            if (booking != null)
            {
                m_bookingService.DeleteBooking(booking);
            }
            return RedirectToAction("Index");
        }

        public IActionResult StudentShowBar()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Student")
            {
                return RedirectToAction("Login", "Auth");
            }

            var vm = new StudentBarViewModel
            {
                Products = m_productService.GetAllProducts(),
                Categories = m_productService.GetAllCategories()
            };
            return PartialView("_StudentBar", vm);
        }

        [HttpGet]
        public IActionResult GetProductDetails(int id)
        {
            var product = m_productService.GetAllProducts().FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            return PartialView("_ProductDetails", product);
        }

        [HttpPost]
        public IActionResult BuyProduct(int productId, int quantity = 1)
        {
            var userEmail = HttpContext.Session.GetString("User");
            var user = m_userService.GetUserByEmail(userEmail);

            var product = m_productService.GetById(productId);
            if (product == null)
                return Json(new { success = false, message = "Produto nao encontrado." });

            decimal totalPrice = product.Price * quantity;

            if (user.Saldo < totalPrice)
            {
                return Json(new
                {
                    success = false,
                    message = $"Saldo insuficiente! Faltam {(totalPrice - user.Saldo):0.00} €"
                });
            }

            if (!m_productService.TryDecrementStock(productId, quantity))
                return Json(new { success = false, message = "Nao tem stock suficiente!" });

            m_userService.AddSaldo(userEmail, -totalPrice);
            var novoSaldo = user.Saldo;

            var order = new Order
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = quantity,
                UserEmail = userEmail
            };

            m_orderService.AddPurchase(order);

            return Json(new
            {
                success = true,
                message = $"Comprado {quantity} de {product.Name}!",
                newStock = product.Stock,
                newSaldo = novoSaldo
            });
        }

        public IActionResult StudentBarHistory()
        {
            var role = HttpContext.Session.GetString("Role");
            var userEmail = HttpContext.Session.GetString("User");
            if (role != "Student")
            {
                return RedirectToAction("Login", "Auth");
            }

            var vm = new StudentBarHistoryViewModel
            {
                Orders = m_orderService.GetByUser(userEmail)
            };
            return PartialView("_StudentPurchaseHistory", vm);
        }

        public IActionResult StudentSaldo()
        {
            var role = HttpContext.Session.GetString("Role");
            var userEmail = HttpContext.Session.GetString("User");
            if (role != "Student")
            {
                return RedirectToAction("Login", "Auth");
            }

            var vm = new SaldoViewModel{
                Saldo = m_userService.GetUserByEmail(userEmail).Saldo,
                UserEmail = userEmail
            };
            return PartialView("_StudentSaldo", vm);
        }

        [HttpPost]
        public IActionResult TopUpSaldo(decimal amount)
        {
            var email = HttpContext.Session.GetString("User");

            if (string.IsNullOrEmpty(email) || amount < 1m)
                return Json(new { success = false, message = "Valor inválido" });

            var user = m_userService.GetUserByEmail(email);
            if (user == null)
                return Json(new { success = false, message = "Utilizador não encontrado" });

            //user.Saldo += amount;
            m_userService.AddSaldo(email, amount);

            return Json(new { success = true, newSaldo = user.Saldo });
        }





        public IActionResult Index()
        {
            //m_productService.LoadProductsFromFile();
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

            var vm = new FuncHomeViewModel
            {
                Products = m_productService.GetAll()
            };

            return View(vm);
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