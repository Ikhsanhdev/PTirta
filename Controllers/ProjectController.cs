using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;
using Higertech.Interfaces;

namespace Higertech.Controllers;

public class ProjectController : Controller
{
    private readonly ILogger<ProjectController> _logger;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public ProjectController( IUnitOfWorkRepository unitOfWorkRepository)
    {
        this._unitOfWorkRepository = unitOfWorkRepository;
    }

    public async Task<IActionResult> Index()
    {

        var model = await _unitOfWorkRepository.Project.GetListProjectAsync();
        return View(model);
    }

    public IActionResult Detail()
    {
       return View();
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
