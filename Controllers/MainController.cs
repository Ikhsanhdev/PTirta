using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;
using Higertech.ViewModels;
using Higertech.Interfaces;
using Higertech.Models.Datatables;
using Serilog;

namespace Higertech.Controllers;

public class MainController : Controller
{
    private readonly ILogger<MainController> _logger;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public MainController(ILogger<MainController> logger,IUnitOfWorkRepository unitOfWorkRepository)
    {
        this._unitOfWorkRepository = unitOfWorkRepository;
        this._logger = logger;
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
            return View();
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex, "Error loading carousel data: {Message}", ex.Message);
            Log.Error(ex, "Error loading carousel data: {Message}", ex.Message);
            return View(new List<Main>());
        }
    }

    // [HttpPost]
    // public async Task<IActionResult> GetMainData()
    // {
    //     try
    //     {
    //         var modelRequest = new JqueryDataTableRequest
    //         {
    //             Draw = Request.Form["draw"].FirstOrDefault() ?? "",
    //             Start = Request.Form["start"].FirstOrDefault() ?? "",
    //             Length = Request.Form["length"].FirstOrDefault() ?? "25",
    //             SortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault() ?? "",
    //             SortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault() ?? "",
    //             SearchValue = Request.Form["search[value]"].FirstOrDefault() ?? "",
    //             Status = Request.Form["status"].FirstOrDefault() ?? ""
    //         };

    //         modelRequest.PageSize = modelRequest.Length == "-1" ? int.MaxValue : Convert.ToInt32(modelRequest.Length);
    //         modelRequest.Skip = Convert.ToInt32(modelRequest.Start);

    //         var (mains, recordsTotal) = await _unitOfWorkRepository.Main.GetDataMain(modelRequest);
            
    //         return Json(new { 
    //             draw = modelRequest.Draw, 
    //             recordsFiltered = recordsTotal, 
    //             recordsTotal = recordsTotal, 
    //             data = mains 
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting data table data: {Message}", ex.Message);
    //         Log.Error(ex, "Error getting data table data: {Message}", ex.Message);
    //         return StatusCode(500, new { error = "Internal server error" });
    //     }
    // }
}
