using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TirtaRK.Models;

namespace TirtaRK.Controllers;

public class TablesController : Controller
{
  public IActionResult Basic() => View();
}
