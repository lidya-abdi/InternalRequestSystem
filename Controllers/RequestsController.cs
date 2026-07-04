using InternalRequestSystem.Models;
using InternalRequestSystem.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InternalRequestSystem.Services;

namespace InternalRequestSystem.Controllers
{
    public class RequestsController : Controller
    {
        private readonly IRequestService _requestService;

        public RequestsController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost]
            public IActionResult Approve(int id)
            {
                if (!CanManageRequests())
                {
                    TempData["ErrorMessage"] = "You are not authorized to perform this action.";
                    return RedirectToAction("Index");
                }

                var request = _requestService.GetById(id);

                if (request == null)
                {
                    return NotFound();
                }

                _requestService.ApproveRequest(id);

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

            var request = _requestService.GetById(id);

            if (request == null)
            {
                return NotFound();
            }

            _requestService.RejectRequest(id);

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

            var request = _requestService.GetById(id);

            if (request == null)
            {
                return NotFound();
            }

            _requestService.MarkRequestInReview(id);

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



        public IActionResult Index(string? searchText, string? statusFilter, int? departmentFilter, int page = 1)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            var role = HttpContext.Session.GetString("Role");
            var email = HttpContext.Session.GetString("Email");

            var requests = _requestService.GetRequestsForUser(
                role,
                email,
                searchText,
                statusFilter,
                departmentFilter);

            ViewBag.SearchText = searchText;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.DepartmentFilter = departmentFilter;
            ViewBag.Departments = _requestService.GetDepartments();
            ViewBag.NotificationCount = _requestService.CountPendingRequests();

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

            ViewBag.Departments = _requestService.GetDepartments();
            ViewBag.NotificationCount = _requestService.CountPendingRequests();

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
                ViewBag.Departments = _requestService.GetDepartments();
                ViewBag.NotificationCount = _requestService.CountPendingRequests();
                return View(request);
            }

            var fullName = HttpContext.Session.GetString("FullName") ?? "";
            var email = HttpContext.Session.GetString("Email") ?? "";

            _requestService.CreateRequest(request, fullName, email);

            TempData["SuccessMessage"] = "Request created successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var request = _requestService.GetDetails(id);

            if (request == null)
            {
                return NotFound();
            }

            ViewBag.RequestLogs = _requestService.GetLogsByRequestId(id);
            ViewBag.NotificationCount = _requestService.CountPendingRequests();

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

            var request = _requestService.GetById(id);

            if (request == null)
            {
                return NotFound();
            }

            ViewBag.NotificationCount = _requestService.CountPendingRequests();

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

            var request = _requestService.GetById(updatedRequest.Id);

            if (request == null)
            {
                return NotFound();
            }

            _requestService.UpdateRequest(updatedRequest);

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

            var request = _requestService.GetById(id);

            if (request == null)
            {
                return NotFound();
            }

            ViewBag.NotificationCount = _requestService.CountPendingRequests();

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

            var request = _requestService.GetForDelete(id);

            if (request == null)
            {
                return NotFound();
            }

            _requestService.DeleteRequest(id);

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

            var logs = _requestService.GetLogs();

            ViewBag.NotificationCount = _requestService.CountPendingRequests();

            return View(logs);
        }
    }
}
