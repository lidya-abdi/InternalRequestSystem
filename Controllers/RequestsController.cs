using Microsoft.AspNetCore.Mvc;
using InternalRequestSystem.Models;

namespace InternalRequestSystem.Controllers
{
    public class RequestsController : Controller
    {
        private static List<Request> requests = new List<Request>();
        /*
         Index  	رح تعرض كل الطلبات
        Create  	رح تعرض صفحة إنشاء طلب  
        */
        public IActionResult Index()
        {
            return View(requests);
        }
        [HttpGet]
        public IActionResult Create()
        {
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
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            request.Id = requests.Count + 1;
            request.Status = "Pending";

            requests.Add(request);

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
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
