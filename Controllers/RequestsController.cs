using Microsoft.AspNetCore.Mvc;
using InternalRequestSystem.Models;

namespace InternalRequestSystem.Controllers
{
    public class RequestsController : Controller
    {
        /*
         Index  	رح تعرض كل الطلبات
        Create  	رح تعرض صفحة إنشاء طلب  
        */
        public IActionResult Index()
        {
            return View();
        }
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
            return RedirectToAction("Index");
        }
    }
}
