using Microsoft.AspNetCore.Mvc;
using InternalRequestSystem.Models;
using Microsoft.AspNetCore.Http;

namespace InternalRequestSystem.Controllers
{
    public class RequestsController : Controller
    {
        private static List<Request> requests = new List<Request>();
        /*
         Index  	رح تعرض كل الطلبات
        Create  	رح تعرض صفحة إنشاء طلب  
        */

        private bool IsUserLoggedIn()
        {
            var fullName = HttpContext.Session.GetString("FullName");
            var email = HttpContext.Session.GetString("Email");

            return !string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(email);
        }

        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            return View(requests);
        }
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

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
                return View(request);
            }

            request.Id = requests.Count + 1;
            request.Status = "Pending";
            request.SubmittedByName = HttpContext.Session.GetString("FullName") ?? "";
            request.SubmittedByEmail = HttpContext.Session.GetString("Email") ?? "";

            requests.Add(request);

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var request = requests.FirstOrDefault(r => r.Id == id);
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

            var request = requests.FirstOrDefault(r => r.Id == id);

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

            var request = requests.FirstOrDefault(r => r.Id == updatedRequest.Id);

            if (request == null)
            {
                return NotFound();
            }

            request.Title = updatedRequest.Title;
            request.Description = updatedRequest.Description;
            request.RequestType = updatedRequest.RequestType;
            request.Status = updatedRequest.Status;

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var request = requests.FirstOrDefault(r => r.Id == id);

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

            var request = requests.FirstOrDefault(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            requests.Remove(request);

            return RedirectToAction("Index");
        }
    }
}
