using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;

namespace Higertech.Controllers;

public class IconsController : Controller
{
  public IActionResult Boxicons() => View();
}
