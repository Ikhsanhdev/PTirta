using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TirtaRK.Models;

namespace TirtaRK.Controllers;

public class IconsController : Controller
{
  public IActionResult Boxicons() => View();
}
