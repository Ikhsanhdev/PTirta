using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TirtaRK.Models;

namespace TirtaRK.Controllers;

public class CardsController : Controller
{
  public IActionResult Basic() => View();
}
