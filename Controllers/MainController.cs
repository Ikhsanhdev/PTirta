using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;
using Higertech.ViewModels;
using Higertech.Interfaces;
using Higertech.Models.Datatables;
using Serilog;
using Higertech.Repositories;

namespace Higertech.Controllers;

public class MainController : Controller
{
    private readonly ILogger<MainController> _logger;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;
    private readonly IProjectRepository _projectRepository;

    public MainController(IUnitOfWorkRepository unitOfWorkRepository, IProjectRepository projectRepository)
    {
        this._unitOfWorkRepository = unitOfWorkRepository;
        this._projectRepository = projectRepository;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult PageNotFound()
    {
        return View("~/Views/404/PageNotFound.cshtml");
    }

    public IActionResult Article()
    {
        return View();
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var mains = await _unitOfWorkRepository.Main.GetAllAsync();
            var projects = await _projectRepository.GetListProjectAsync();

            if (mains == null || !mains.Any())
            {
                _logger.LogWarning("No data found");
                return View(new MainViewModel());
            }

            var viewModel = new MainViewModel
            {
                Posters = mains.Where(m => m.Category == "poster").ToList(),
                Tombol = mains.Where(m => m.Category == "tombol").ToList(),
                Projects = projects.OrderByDescending(p => p.UpdatedAt).Take(6).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading data: {Message}", ex.Message);
            return View(new MainViewModel());
        }
    }
}
