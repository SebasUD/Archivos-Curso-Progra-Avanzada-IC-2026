using Microsoft.AspNetCore.Mvc;

namespace MyNewMvcProject.Controllers;

public class InitialController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult HelloWorld()
    {
        return View();
    }

    [HttpPost]
    public IActionResult HelloWorld(string name)
    {
        ViewData["UserName"] = name;
        return View();
    }
}
