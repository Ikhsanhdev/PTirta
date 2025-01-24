using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;

namespace Higertech.Controllers;

public class CardsController : Controller
{
  public IActionResult Basic() => View();
}
