using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;

namespace Higertech.Controllers;

public class DashboardsController : Controller
{
  public IActionResult Index() => View();
}
