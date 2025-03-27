using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TirtaRK.Models;

namespace TirtaRK.Controllers;

public class FormsController : Controller
{
  public IActionResult BasicInputs() => View();
  public IActionResult InputGroups() => View();
}
