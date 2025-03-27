using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TirtaRK.Models;

namespace TirtaRK.Controllers;

public class DashboardsController : Controller
{
  public IActionResult Index() => View();
}
