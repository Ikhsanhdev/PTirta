using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TirtaRK.Models;
using TirtaRK.ViewModels;
using TirtaRK.Interfaces;
using TirtaRK.Models.Datatables;
using Serilog;

namespace TirtaRK.Controllers;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public ProductController(IUnitOfWorkRepository unitOfWorkRepository)
    {
        this._unitOfWorkRepository = unitOfWorkRepository;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var products = await _unitOfWorkRepository.Product.GetAllAsync();
            return View(products);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading products: {Message}", ex.Message);
            return View(new List<Product>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        try
        {
            var product = await _unitOfWorkRepository.Product.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Json(product);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting product details: {Message}", ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetProductData()
    {
        var modelRequest = new JqueryDataTableRequest
        {
            Draw = Request.Form["draw"].FirstOrDefault() ?? "",
            Start = Request.Form["start"].FirstOrDefault() ?? "",
            Length = Request.Form["length"].FirstOrDefault() ?? "25",
            SortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault() ?? "",
            SortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault() ?? "",
            SearchValue = Request.Form["search[value]"].FirstOrDefault() ?? "",
            Status = Request.Form["status"].FirstOrDefault() ?? ""
        };

        try
        {
            modelRequest.PageSize = modelRequest.Length == "-1" ? int.MaxValue : Convert.ToInt32(modelRequest.Length);
            modelRequest.Skip = Convert.ToInt32(modelRequest.Start);

            var (products, recordsTotal) = await _unitOfWorkRepository.Product.GetDataProduct(modelRequest);
            return Json(new { 
                draw = modelRequest.Draw, 
                recordsFiltered = recordsTotal, 
                recordsTotal = recordsTotal, 
                data = products 
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting product data: {Message}", ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> CheckDuplicateName(string productName, Guid? id = null)
    {
        try
        {
            var exists = await _unitOfWorkRepository.Product.IsProductNameExistsAsync(productName, id);
            return Json(new { isDuplicate = exists });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error checking duplicate product name: {Message}", ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }
}