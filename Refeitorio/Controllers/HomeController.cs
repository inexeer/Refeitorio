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
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

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

            var chosenVeg = model.SelectedVegetarianId.HasValue
                ? m_menuService.GetAll().FirstOrDefault(m => m.Id == model.SelectedVegetarianId.Value)
                : null;
            var chosenNorm = model.SelectedNormalId.HasValue
                ? m_menuService.GetAll().FirstOrDefault(m => m.Id == model.SelectedNormalId.Value)
                : null;

            if (chosenNorm == null && chosenVeg == null)
            {
                TempData["Error"] = "Seleciona pelo menos um menu.";
                return RedirectToAction("Index");
            }

            var dateOnly = model.Data;

            if (chosenNorm != null)
                m_menuService.SaveLunch(chosenNorm, dateOnly);

            if (chosenVeg != null)
                m_menuService.SaveLunch(chosenVeg, dateOnly);

            m_menuService.SerializarDict();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetSavedLunches()
        {
            var data = m_menuService.LunchByDate
                .ToDictionary(
                    k => k.Key.ToString("yyyy-MM-dd"),
                    v => v.Value.ToDictionary(
                        opt => opt.Key.ToString(),
                        day => new
                        {
                            mainDish = day.Value.MainDish ?? "",
                            soup = day.Value.Soup ?? "",
                            dessert = day.Value.Dessert ?? "",
                            option = day.Value.Option.ToString()
                        }
                    )
                );

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
        public IActionResult CreateProduct(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Id = m_productService.GetAll().Any()
            ? m_productService.GetAll().Max(p => p.Id) + 1
            : 1,
                    Category = model.Category,
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Kcal = model.Kcal,
                    Protein = model.Protein,
                    Fat = model.Fat,
                    Carbs = model.Carbs,
                    Allergens = model.Allergens,
                    Stock = model.Stock,
                    ImageFileName = "default.jpg"
                };

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var ext = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                    if (!allowed.Contains(ext))
                    {
                        ModelState.AddModelError("ImageFile", "Formato inválido.");
                        return RedirectToAction("Index");
                    }

                    product.ImageFileName = $"{product.Id}{ext}";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "productImages", product.ImageFileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        model.ImageFile.CopyTo(stream);
                    }
                }

                m_productService.AddProduct(product);
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
            var vm = new EditProductViewModel
            {
                Id = product.Id,
                Category = product.Category,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Kcal = product.Kcal,
                Protein = product.Protein,
                Fat = product.Fat,
                Carbs = product.Carbs,
                Allergens = product.Allergens,
                Stock = product.Stock,
                CurrentImageFileName = product.ImageFileName
            };
            return PartialView("_FuncEditProduct", vm);
        }

        [HttpPost]
        public IActionResult EditProduct(EditProductViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = m_productService.GetById(vm.Id);
                if (existingProduct == null) return NotFound();

                existingProduct.Category = vm.Category;
                existingProduct.Name = vm.Name;
                existingProduct.Description = vm.Description;
                existingProduct.Price = vm.Price;
                existingProduct.Kcal = vm.Kcal;
                existingProduct.Protein = vm.Protein;
                existingProduct.Fat = vm.Fat;
                existingProduct.Carbs = vm.Carbs;
                existingProduct.Allergens = vm.Allergens;
                existingProduct.Stock = vm.Stock;

                if (vm.NewImageFile != null && vm.NewImageFile.Length > 0)
                {
                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var ext = Path.GetExtension(vm.NewImageFile.FileName).ToLowerInvariant();

                    if (!allowed.Contains(ext))
                    {
                        ModelState.AddModelError("NewImageFile", "Formato inválido.");
                        return View(vm);
                    }

                    var newFileName = $"{vm.Id}{ext}";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "productImages", newFileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        vm.NewImageFile.CopyTo(stream);
                    }

                    existingProduct.ImageFileName = newFileName;
                }

                m_productService.UpdateProduct(existingProduct);
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
        public IActionResult BookLunch(string date, string type)
        {
            var email = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(email))
                return Json(new { success = false, message = "Nao autenticado" });

            if (!Enum.TryParse<MenuOption>(type, out var menuOpt))
                return Json(new { success = false, message = "Opcao de menu invalida" });

            DateOnly parsedDate;
            try
            {
                parsedDate = DateOnly.ParseExact(date, "yyyy-MM-dd");
            }
            catch
            {
                return Json(new { success = false, message = $"Data invalida: '{date}'" });
            }

            if (!m_menuService.LunchByDate.TryGetValue(parsedDate, out var optDict))
            {
                return Json(new { success = false, message = "Nao ha menu neste dia" });
            }
            if (!optDict.TryGetValue(menuOpt, out var menu))
            {
                return Json(new { success = false, message = "Nao ha menu desta opcao neste dia" });
            }

            m_bookingService.BookLunch(email, date, menuOpt.ToString());

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

            m_userService.AddSaldo(email, amount);

            return Json(new { success = true, newSaldo = user.Saldo });
        }





        public IActionResult Index()
        {

            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || (role != "Admin" && role != "Staff" && role != "Student"))
            {
                return RedirectToAction("Login", "Auth");
            }

            var userEmail = HttpContext.Session.GetString("User");
            if (!string.IsNullOrEmpty(userEmail))
            {
                var user = m_user_service_get(userEmail);
                if (user != null)
                {
                    ViewBag.UserName = user.FullName;

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

        [HttpPost]
        public IActionResult RejectUser(Guid id)
        {
            m_userService.RejectUser(id);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private Refeitorio.Models.User? m_user_service_get(string email)
        {
            return m_userService.GetUserByEmail(email);
        }
    }
}