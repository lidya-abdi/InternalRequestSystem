using Microsoft.AspNetCore.Mvc;
using InternalRequestSystem.Models;
using Microsoft.AspNetCore.Http;

namespace InternalRequestSystem.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            // Store user info in session
            HttpContext.Session.SetString("FullName", model.FullName);
            HttpContext.Session.SetString("Email", model.Email);

            // Redirect to the request submission page after successful login
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Logout()
        {
            // Clear the session to log out the user
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
