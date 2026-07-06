using Microsoft.AspNetCore.Mvc;
using InternalRequestSystem.Models;
using Microsoft.AspNetCore.Http;
using InternalRequestSystem.Data;
using Microsoft.EntityFrameworkCore;
using InternalRequestSystem.Services;

namespace InternalRequestSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

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

            var email = model.Email.Trim().ToLower();

            if (!email.EndsWith("@internal.com"))
            {
                ModelState.AddModelError("", "Email must be an internal company email.");
                return View(model);
            }

            var emailParts = email.Split('@')[0].Split('.');

            if (emailParts.Length != 2)
            {
                ModelState.AddModelError("", "Email format must be name.role@internal.com.");
                return View(model);
            }

            var emailName = emailParts[0];
            var role = emailParts[1];

            if (!string.Equals(model.FullName.Trim(), emailName, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Name does not match the email owner.");
                return View(model);
            }

            if (role != "employee" && role != "manager" && role != "admin")
            {
                ModelState.AddModelError("", "Role in email must be employee, manager, or admin.");
                return View(model);
            }

            var formattedRole =
                role == "employee" ? "Employee" :
                role == "manager" ? "Manager" :
                "Admin";

            var user = _context.AppUsers
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email.ToLower() == email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found in the system.");
                return View(model);
            }

            var token = _tokenService.GenerateToken(user);
            TempData["JwtToken"] = token;

            HttpContext.Session.SetString("FullName", model.FullName.Trim());
            HttpContext.Session.SetString("Email", model.Email.Trim());
            HttpContext.Session.SetString("Role", formattedRole);

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
