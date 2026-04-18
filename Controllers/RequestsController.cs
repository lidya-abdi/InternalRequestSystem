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
            request.Status = "Panding";

            requests.Add(request);

            return RedirectToAction("Index");
        }
    }
}
