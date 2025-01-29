using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGateway.Controllers;

[Authorize("Admin,Comptable")]
public class Facturation : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}