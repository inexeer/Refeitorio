using Microsoft.AspNetCore.Mvc;

namespace Refeitorio.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
