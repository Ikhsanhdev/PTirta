using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;

namespace Higertech.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;

    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
     public IActionResult Article()
    {
        return View("~/Views/Admin/Article/Index.cshtml");
    }
      public IActionResult CreateEdit()
    {
        return View("~/Views/Admin/Article/CreateEdit.cshtml");
    }
}
