using LoggingWebSite.Models;
using Microsoft.AspNet.Mvc;

namespace LoggingWebSite.Controllers
{
    public class EmployeesController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit(int id, Employee employee)
        {
            return View();
        }
    }
}
