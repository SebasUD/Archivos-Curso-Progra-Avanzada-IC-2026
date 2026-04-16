using Microsoft.AspNetCore.Mvc;

namespace ParkingSystemApp.Controllers;

public class AutomobilesMasterDetailController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
