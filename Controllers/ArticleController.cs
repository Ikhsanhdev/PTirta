using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;

namespace Higertech.Controllers;

public class ArticleController : Controller
{
    private readonly ILogger<ArticleController> _logger;

    public ArticleController(ILogger<ArticleController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Detail()
    {
       return View();
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
