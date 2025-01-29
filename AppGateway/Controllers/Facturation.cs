using Microsoft.AspNetCore.Mvc;

namespace AppGateway.Controllers;

public class Facturation : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}