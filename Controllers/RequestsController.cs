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

            [HttpPost]
            public IActionResult Approve(int id)
            {
                if (!CanManageRequests())
                {
                    TempData["ErrorMessage"] = "You are not authorized to perform this action.";
                    return RedirectToAction("Index");
                }

                var request = _context.Requests.FirstOrDefault(r => r.Id == id);

                if (request == null)
                {
                    return NotFound();
                }

                request.Status = "Approved";

                var approval = new Approval
                {
                    RequestId = request.Id,
                    Decision = "Approved",
                    ApprovedByUserId = null,
                    Comment = "Request approved successfully."
                };

                _context.Approvals.Add(approval);

                AddRequestLog(
                    request.Id,
                    "Approved",
                    $"Request '{request.Title}' was approved."
                );

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Request approved successfully.";
    
                return RedirectToAction("Index");
            }

            [HttpPost]
            public IActionResult Reject(int id)
            {
                if (!CanManageRequests())
                {
                    TempData["ErrorMessage"] = "You are not authorized to perform this action.";
                    return RedirectToAction("Index");
                }

                var request = _context.Requests.FirstOrDefault(r => r.Id == id);

                if (request == null)
                {
                    return NotFound();
                }

                request.Status = "Rejected";

                var approval = new Approval
                {
                    RequestId = request.Id,
                    Decision = "Rejected",
                    ApprovedByUserId = null,
                    Comment = "Request rejected."
                };

                _context.Approvals.Add(approval);

                AddRequestLog(
                    request.Id,
                    "Rejected",
                    $"Request '{request.Title}' was rejected."
                );

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Request rejected successfully.";

                return RedirectToAction("Index");
            }

        [HttpPost]
        public IActionResult MarkInReview(int id)
        {
            if (!CanManageRequests())
            {
                TempData["ErrorMessage"] = "You are not authorized to perform this action.";
                return RedirectToAction("Index");
            }

            var request = _context.Requests.FirstOrDefault(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = "In Review";

            AddRequestLog(
                request.Id,
                "In Review",
                $"Request '{request.Title}' moved to review stage."
            );

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Request moved to review successfully.";

            return RedirectToAction("Index");
        }

        private bool IsUserLoggedIn()
        {
            var fullName = HttpContext.Session.GetString("FullName");
            var email = HttpContext.Session.GetString("Email");
            var role = HttpContext.Session.GetString("Role");

            return !string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(role);
        }

        private bool CanManageRequests()
        {
            var role = HttpContext.Session.GetString("Role");

            return role == "Manager" || role == "Admin";
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

        public IActionResult Index(string? searchText, string? statusFilter, int? departmentFilter, int page = 1)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            var requests = _context.Requests
                .Include(r => r.Department)
                .AsQueryable();

            var role = HttpContext.Session.GetString("Role");
            var email = HttpContext.Session.GetString("Email");

            if (role == "Employee")
            {
                requests = requests.Where(r => r.SubmittedByEmail == email);
            }

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
            ViewBag.NotificationCount = _context.Requests.Count(r => r.Status == "Pending");

            int pageSize = 5;

            var totalItems = requests.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedRequests = requests
                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedRequests);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.NotificationCount = _context.Requests.Count(r => r.Status == "Pending");

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
            request.CreatedDate = DateTime.Now;
            request.SubmittedByName = HttpContext.Session.GetString("FullName") ?? "";
            request.SubmittedByEmail = HttpContext.Session.GetString("Email") ?? "";

            _context.Requests.Add(request);
            _context.SaveChanges();

            AddRequestLog(request.Id, "Created", $"Request '{request.Title}' was created.");
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Request created successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var request = _context.Requests
                .Include(r => r.Department)
                .FirstOrDefault(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            ViewBag.RequestLogs = _context.RequestLogs
                .Where(l => l.RequestId == id)
                .OrderByDescending(l => l.ActionDate)
                .ToList();
            ViewBag.NotificationCount = _context.Requests.Count(r => r.Status == "Pending");

            return View(request);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!CanManageRequests())
            {
                TempData["ErrorMessage"] = "You are not authorized to edit requests.";
                return RedirectToAction("Index");
            }

            var request = _context.Requests.FirstOrDefault(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            ViewBag.NotificationCount = _context.Requests.Count(r => r.Status == "Pending");

            return View(request);
        }
        [HttpPost]
        public IActionResult Edit(Request updatedRequest)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!CanManageRequests())
            {
                TempData["ErrorMessage"] = "You are not authorized to edit requests.";
                return RedirectToAction("Index");
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
            

            AddRequestLog(request.Id, "Updated", $"Request '{request.Title}' was updated.");

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Request updated successfully.";

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!CanManageRequests())
            {
                TempData["ErrorMessage"] = "You are not authorized to delete requests.";
                return RedirectToAction("Index");
            }

            var request = _context.Requests.FirstOrDefault(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            ViewBag.NotificationCount = _context.Requests.Count(r => r.Status == "Pending");

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!CanManageRequests())
            {
                TempData["ErrorMessage"] = "You are not authorized to delete requests.";
                return RedirectToAction("Index");
            }

            var request = _context.Requests.Include(r => r.Approvals).FirstOrDefault(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            AddRequestLog(request.Id, "Deleted", $"Request '{request.Title}' was deleted.");

            if (request.Approvals != null && request.Approvals.Any())
            {
                _context.Approvals.RemoveRange(request.Approvals);
            }

            _context.Requests.Remove(request);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Request deleted successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Logs()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!CanManageRequests())
            {
                TempData["ErrorMessage"] = "You are not authorized to view logs.";
                return RedirectToAction("Index");
            }

            var logs = _context.RequestLogs
                .OrderByDescending(l => l.ActionDate)
                .ToList();

            ViewBag.NotificationCount = _context.Requests.Count(r => r.Status == "Pending");

            return View(logs);
        }
    }
}
