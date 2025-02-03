using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;

namespace Higertech.Controllers;

public class ProdukController : Controller
{
    private readonly ILogger<ProdukController> _logger;

    public ProdukController(ILogger<ProdukController> logger)
    {
        _logger = logger;
    }

    public IActionResult Produk()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}