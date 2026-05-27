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
            ViewBag.FullName = HttpContext.Session.GetString("FullName") ?? "User";

            ViewBag.TotalRequests = _context.Requests.Count();
            ViewBag.PendingRequests = _context.Requests.Count(r => r.Status == "Pending");
            ViewBag.ApprovedRequests = _context.Requests.Count(r => r.Status == "Approved");
            ViewBag.RejectedRequests = _context.Requests.Count(r => r.Status == "Rejected");
            ViewBag.DepartmentsCount = _context.Departments.Count();

            ViewBag.LatestRequests = _context.Requests
                .Include(r => r.Department)
                .OrderByDescending(r => r.Id)
                .Take(5)
                .ToList();

            ViewBag.LatestActivities = _context.RequestLogs
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