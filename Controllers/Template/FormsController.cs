using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;

namespace Higertech.Controllers;

public class FormsController : Controller
{
  public IActionResult BasicInputs() => View();
  public IActionResult InputGroups() => View();
}
