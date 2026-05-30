using System.Diagnostics;
using InternalRequestSystem.Data;
using InternalRequestSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternalRequestSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var fullName = HttpContext.Session.GetString("FullName") ?? "User";
            var email = HttpContext.Session.GetString("Email");
            var role = HttpContext.Session.GetString("Role");

            ViewBag.FullName = fullName;

            var requestsQuery = _context.Requests
                .Include(r => r.Department)
                .AsQueryable();

            if (role == "Employee")
            {
                requestsQuery = requestsQuery
                    .Where(r => r.SubmittedByEmail.ToLower() == email.ToLower());
            }

            ViewBag.TotalRequests = requestsQuery.Count();
            ViewBag.PendingRequests = requestsQuery.Count(r => r.Status == "Pending");
            ViewBag.NotificationCount = requestsQuery.Count(r => r.Status == "Pending");
            ViewBag.ApprovedRequests = requestsQuery.Count(r => r.Status == "Approved");
            ViewBag.RejectedRequests = requestsQuery.Count(r => r.Status == "Rejected");
            ViewBag.DepartmentsCount = _context.Departments.Count();

            ViewBag.LatestRequests = requestsQuery
                .OrderByDescending(r => r.Id)
                .Take(5)
                .ToList();

            var requestIds = requestsQuery.Select(r => r.Id).ToList();

            ViewBag.LatestActivities = _context.RequestLogs
                .Where(l => role != "Employee" ||
                            (l.RequestId != null && requestIds.Contains(l.RequestId.Value)))
                .OrderByDescending(l => l.ActionDate)
                .Take(5)
                .ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}