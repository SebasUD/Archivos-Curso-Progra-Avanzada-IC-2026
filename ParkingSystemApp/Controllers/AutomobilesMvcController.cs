using Microsoft.AspNetCore.Mvc;

namespace ParkingSystemApp.Controllers;

public class AutomobilesMvcController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
