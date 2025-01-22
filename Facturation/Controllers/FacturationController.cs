using Microsoft.AspNetCore.Mvc;

namespace Facturation.Controllers;

public class FacturationController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}