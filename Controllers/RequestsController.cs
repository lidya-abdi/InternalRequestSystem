using InternalRequestSystem.Data;
using InternalRequestSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternalRequestSystem.Controllers
{
    public class RequestsController : Controller
    {
        private readonly AppDbContext _context;

        public RequestsController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsUserLoggedIn()
        {
            var fullName = HttpContext.Session.GetString("FullName");
            var email = HttpContext.Session.GetString("Email");

            return !string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(email);
        }

        private void AddRequestLog(int? requestId, string action, string description)
        {
            var log = new RequestLog
            {
                RequestId = requestId,
                Action = action,
                Description = description,
                PerformedBy = HttpContext.Session.GetString("FullName") ?? "Unknown User",
                ActionDate = DateTime.Now
            };

            _context.RequestLogs.Add(log);
        }

        public IActionResult Index(string? searchText, string? statusFilter, int? departmentFilter)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            var requests = _context.Requests
                .Include(r => r.Department)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                requests = requests.Where(r =>
                    r.Title.Contains(searchText) ||
                    r.Description.Contains(searchText) ||
                    r.RequestType.Contains(searchText));
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                requests = requests.Where(r => r.Status == statusFilter);
            }

            if (departmentFilter.HasValue)
            {
                requests = requests.Where(r => r.DepartmentId == departmentFilter.Value);
            }

            ViewBag.SearchText = searchText;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.DepartmentFilter = departmentFilter;
            ViewBag.Departments = _context.Departments.ToList();

            return View(requests.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Departments = _context.Departments.ToList();

            return View();
        }
        /*
         ***صفحة Create***
         فورم
         Controller يستقبل البيانات
        */
        [HttpPost]
        public IActionResult Create(Request request)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View(request);
            }

            request.Status = "Pending";
            request.SubmittedByName = HttpContext.Session.GetString("FullName") ?? "";
            request.SubmittedByEmail = HttpContext.Session.GetString("Email") ?? "";

            _context.Requests.Add(request);
            _context.SaveChanges();

            AddRequestLog(request.Id, "Created", $"Request '{request.Title}' was created.");
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var request = _context.Requests.FirstOrDefault(r => r.Id == id);
            if (request == null)
            {
                return NotFound();
            }
            return View(request);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var request = _context.Requests.FirstOrDefault(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }
            return View(request);
        }
        [HttpPost]
        public IActionResult Edit(Request updatedRequest)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(updatedRequest);
            }

            var request = _context.Requests.FirstOrDefault(r => r.Id == updatedRequest.Id);

            if (request == null)
            {
                return NotFound();
            }

            request.Title = updatedRequest.Title;
            request.Description = updatedRequest.Description;
            request.RequestType = updatedRequest.RequestType;
            request.Status = updatedRequest.Status;

            AddRequestLog(request.Id, "Updated", $"Request '{request.Title}' was updated.");

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var request = _context.Requests.FirstOrDefault(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var request = _context.Requests.FirstOrDefault(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            AddRequestLog(request.Id, "Deleted", $"Request '{request.Title}' was deleted.");

            _context.Requests.Remove(request);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Logs()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var logs = _context.RequestLogs
                .OrderByDescending(l => l.ActionDate)
                .ToList();

            return View(logs);
        }
    }
}
