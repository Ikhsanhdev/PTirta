using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TirtaRK.Models;

namespace TirtaRK.Controllers;

public class ExtendedUiController : Controller
{
  public IActionResult PerfectScrollbar() => View();
  public IActionResult TextDivider() => View();
}
