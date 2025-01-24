using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;

namespace Higertech.Controllers;

public class FormLayoutsController : Controller
{
public IActionResult Horizontal() => View();
public IActionResult Vertical() => View();
}
