using Microsoft.AspNetCore.Mvc;

namespace Refeitorio.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
    }
}
