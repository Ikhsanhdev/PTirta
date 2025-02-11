using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;
using Higertech.ViewModels;
using Higertech.Interfaces;
using Higertech.Models.Datatables;
using Serilog;

namespace Higertech.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public AdminController(IUnitOfWorkRepository unitOfWorkRepository)
    {
        this._unitOfWorkRepository = unitOfWorkRepository;
    }


    public IActionResult Index()
    {
        return View();
    }
   

    #region  <=================================== Article ========================================>

    public IActionResult Article()
    {
        return View("~/Views/Admin/Article/Index.cshtml");
    }

    [HttpGet("article/create")]
    public IActionResult CreateEditArticle(Guid id)
    {
        ArticleVM model = new ArticleVM();

        return View("~/Views/Admin/Article/CreateEdit.cshtml", model);
    }


    [HttpGet("article/edit/{id}")]
    public IActionResult EditArticle(Guid id)
    {
        ArticleVM model = new ArticleVM();

        model = _unitOfWorkRepository.Article.GetArticleByIdAsync(id).Result;

        return View("~/Views/Admin/Article/CreateEdit.cshtml", model);
    }

    [HttpPost]
    [Route("/Admin/Article")]
    public async Task<IActionResult> SaveArticle(ArticleVM model)
    {
        AjaxResponse response = new();
        response = await _unitOfWorkRepository.Article.SaveAsync(model);
        return Json(response);
    }

    [HttpDelete]
    [Route("/article/delete/{id}")]
    public async Task<IActionResult> DeleteArticle(Guid id)
    {
        AjaxResponse response = new();
        var msg = await _unitOfWorkRepository.Article.DeleteArticleAsync(id);

        if (msg)
        {
            response.Message = "Data berhasil dihapus";
            response.Code = 200;
        }
        else
        {
            response.Message = "Data gagal dihapus";
            response.Code = 500;
        }

        return Json(response);
    }

    public async Task<IActionResult> GetArticleData()
    {
        var ModelRequest = new JqueryDataTableRequest
        {
            Draw = Request.Form["draw"].FirstOrDefault() ?? "",
            Start = Request.Form["start"].FirstOrDefault() ?? "",
            Length = Request.Form["length"].FirstOrDefault() ?? "25",
            SortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault() ?? "",
            SortColumnDirection = Request.Form["order[0]dir"].FirstOrDefault() ?? "",
            SearchValue = Request.Form["search_value"].FirstOrDefault() ?? "",
            Status = Request.Form["status"].FirstOrDefault() ?? ""

        };

        try
        {
            if (ModelRequest.Length == "-1")
            {
                ModelRequest.PageSize = int.MaxValue;
            }
            else
            {
                ModelRequest.PageSize = ModelRequest.PageSize != null ? Convert.ToInt32(ModelRequest.Length) : 0;
            }

            ModelRequest.Skip = ModelRequest.Start != null ? Convert.ToInt32(ModelRequest.Start) : 0;

            var (rekomendasi, recordsTotal) = await _unitOfWorkRepository.Article.GetDataArticle(ModelRequest);
            var jsonData = new { draw = ModelRequest.Draw, recordsFiltered = recordsTotal, recordsTotal, data = rekomendasi };
            return Json(jsonData);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, DatatableRequest = ModelRequest });
            throw;
        }
    }
    #endregion
    #region  <=================================== Project ========================================>
    public IActionResult Project()
    {
        return View("~/Views/Admin/Project/Index.cshtml");
    }

    [HttpGet("project/create")]
    public IActionResult CreateEditProject()
    {
        ProjectVM model = new ProjectVM();
        return View("~/Views/Admin/Project/CreateEdit.cshtml",model);
    }

    [HttpGet("project/edit/{id}")]
    public IActionResult EditProject(Guid id)
    {
        ProjectVM model = new ProjectVM();

        model = _unitOfWorkRepository.Project.GetProjectByIdAsync(id).Result;

        return View("~/Views/Admin/Project/CreateEdit.cshtml", model);
    }

    [HttpPost]
    [Route("/Admin/Project")]
    public async Task<IActionResult> SaveProject(ProjectVM model)
    {
        AjaxResponse response = new();
        response = await _unitOfWorkRepository.Project.SaveAsync(model);
        return Json(response);
    }

    [HttpDelete]
    [Route("/project/delete/{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        AjaxResponse response = new();
        var msg = await _unitOfWorkRepository.Project.DeleteAsync(id);

        if (msg)
        {
            response.Message = "Data berhasil dihapus";
            response.Code = 200;
        }
        else
        {
            response.Message = "Data gagal dihapus";
            response.Code = 500;
        }

        return Json(response);
    }

    public async Task<IActionResult> GetProjectData()
    {
        var ModelRequest = new JqueryDataTableRequest
        {
            Draw = Request.Form["draw"].FirstOrDefault() ?? "",
            Start = Request.Form["start"].FirstOrDefault() ?? "",
            Length = Request.Form["length"].FirstOrDefault() ?? "25",
            SortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault() ?? "",
            SortColumnDirection = Request.Form["order[0]dir"].FirstOrDefault() ?? "",
            SearchValue = Request.Form["search_value"].FirstOrDefault() ?? "",
            Status = Request.Form["status"].FirstOrDefault() ?? ""

        };

        try
        {
            if (ModelRequest.Length == "-1")
            {
                ModelRequest.PageSize = int.MaxValue;
            }
            else
            {
                ModelRequest.PageSize = ModelRequest.PageSize != null ? Convert.ToInt32(ModelRequest.Length) : 0;
            }

            ModelRequest.Skip = ModelRequest.Start != null ? Convert.ToInt32(ModelRequest.Start) : 0;

            var (rekomendasi, recordsTotal) = await _unitOfWorkRepository.Project.GetDataProject(ModelRequest);
            var jsonData = new { draw = ModelRequest.Draw, recordsFiltered = recordsTotal, recordsTotal, data = rekomendasi };
            return Json(jsonData);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, DatatableRequest = ModelRequest });
            throw;
        }
    }
    #endregion
    #region  <=================================== Product ========================================>
    public IActionResult Product()
    {
        return View("~/Views/Admin/Product/Index.cshtml");
    }

    public async Task<IActionResult> GetProductData()
    {
        var ModelRequest = new JqueryDataTableRequest
        {
            Draw = Request.Form["draw"].FirstOrDefault() ?? "",
            Start = Request.Form["start"].FirstOrDefault() ?? "",
            Length = Request.Form["length"].FirstOrDefault() ?? "25",
            SortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault() ?? "",
            SortColumnDirection = Request.Form["order[0]dir"].FirstOrDefault() ?? "",
            SearchValue = Request.Form["search_value"].FirstOrDefault() ?? "",
            Status = Request.Form["status"].FirstOrDefault() ?? ""

        };

        try
        {
            if (ModelRequest.Length == "-1")
            {
                ModelRequest.PageSize = int.MaxValue;
            }
            else
            {
                ModelRequest.PageSize = ModelRequest.PageSize != null ? Convert.ToInt32(ModelRequest.Length) : 0;
            }

            ModelRequest.Skip = ModelRequest.Start != null ? Convert.ToInt32(ModelRequest.Start) : 0;

            var (rekomendasi, recordsTotal) = await _unitOfWorkRepository.Product.GetDataProduct(ModelRequest);
            var jsonData = new { draw = ModelRequest.Draw, recordsFiltered = recordsTotal, recordsTotal, data = rekomendasi };
            return Json(jsonData);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, DatatableRequest = ModelRequest });
            throw;
        }
    }
    #endregion
}

