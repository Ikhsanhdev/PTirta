using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Higertech.Models;
using Higertech.ViewModels;
using Higertech.Interfaces;
using Higertech.Models.Datatables;
using Serilog;
using System.Threading.Tasks;

namespace Higertech.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;
    private readonly IUnitOfWorkService _unitOfWorkService;

    

    public AdminController(IUnitOfWorkRepository unitOfWorkRepository, IUnitOfWorkService unitOfWorkService)
    {
        this._unitOfWorkRepository = unitOfWorkRepository;
        this._unitOfWorkService = unitOfWorkService;
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

        var model = _unitOfWorkRepository.Article.GetArticleByIdAsync(id).Result;
        if (model == null)
        {
            return View("~/Views/404/PageNotFound.cshtml");
        }
        return View("~/Views/Admin/Article/CreateEdit.cshtml", model);
    }

    [HttpPost]
    [Route("/Admin/Article")]
    public async Task<IActionResult> SaveArticle(ArticleVM model,IFormFile file)
    {
        AjaxResponse response = new();
        if (file != null)
        {
            model.img_url = await _unitOfWorkService.ImageUploads.UploadImageAsync(file, "articles");
        }
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
        
        return View("~/Views/Admin/Project/CreateEdit.cshtml", model);
    }

    [HttpGet("project/edit/{id}")]
    public IActionResult EditProject(Guid id)
    {

        var model = _unitOfWorkRepository.Project.GetProjectByIdAsync(id).Result;

        if (model == null)
        {
            return View("~/Views/404/PageNotFound.cshtml");
        }
        return View("~/Views/Admin/Project/CreateEdit.cshtml", model);
    }

    [HttpPost]
    [Route("/Admin/Project")]
    public async Task<IActionResult> SaveProject(ProjectVM model,IFormFile file)
    {
        AjaxResponse response = new();

        if (file != null)
        {
            model.img_url = await _unitOfWorkService.ImageUploads.UploadImageAsync(file, "projects");
        }

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

    [HttpGet("product/create")]
    public IActionResult CreateEditProduct()
    {
        ProductVM model = new ProductVM();
        return View("~/Views/Admin/Product/CreateEdit.cshtml",model);
    }

    [HttpPost]
    [Route("/Admin/Product")]
    public async Task<IActionResult> SaveProduct(ProductVM model)
    {
        AjaxResponse response = new();
        response = await _unitOfWorkRepository.Product.SaveAsync(model);
        return Json(response);
    }

    [HttpGet("product/edit/{id}")]
    public IActionResult EditProduct(Guid id)
    {
        ProductVM model = new ProductVM();

        model = _unitOfWorkRepository.Product.GetProductByIdAsync(id).Result;

        return View("~/Views/Admin/Product/CreateEdit.cshtml", model);
    }

    [HttpDelete]
    [Route("/product/delete/{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        AjaxResponse response = new();
        var msg = await _unitOfWorkRepository.Product.DeleteProductAsync(id);

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
    #endregion
    #region <=================================== Home ========================================>
    public IActionResult Main()
    {
        return View("~/Views/Admin/Main/Index.cshtml");
    }

    public async Task<IActionResult> GetMainData()
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

            var (rekomendasi, recordsTotal) = await _unitOfWorkRepository.Main.GetDataMain(ModelRequest);
            var jsonData = new { draw = ModelRequest.Draw, recordsFiltered = recordsTotal, recordsTotal, data = rekomendasi };
            return Json(jsonData);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, DatatableRequest = ModelRequest });
            throw;
        }
    }

    [HttpGet("main/create")]
    public IActionResult CreateEditMain(Guid id)
    {
        MainVM model = new MainVM();

        return View("~/Views/Admin/Main/CreateEdit.cshtml", model);
    }
    
    [HttpPost]
    [Route("/Admin/Main")]
    public async Task<IActionResult> SaveMain([FromForm] MainVM model, IFormFile file)
    {
        try 
        {
            if (file != null)
            {
                model.img_url = await _unitOfWorkService.ImageUploads.UploadImageAsync(file, "main");
            }
            
            var response = await _unitOfWorkRepository.Main.SaveAsync(model);
            return Json(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving main data: {@ExceptionDetails}", 
                new { ex.Message, ex.StackTrace });
            return Json(new AjaxResponse 
            { 
                Code = 500, 
                Message = "Terjadi kesalahan saat menyimpan data" 
            });
        }
    }

    [HttpGet("main/edit/{id}")]
    public IActionResult EditMain(Guid id)
    {

        var model = _unitOfWorkRepository.Main.GetMainByIdAsync(id).Result;
        if (model == null)
        {
            return View("~/Views/404/PageNotFound.cshtml");
        }
        return View("~/Views/Admin/Main/CreateEdit.cshtml", model);
    }

    [HttpDelete]
    [Route("/main/delete/{id}")]
    public async Task<IActionResult> DeleteMain(Guid id)
    {
        AjaxResponse response = new();
        var msg = await _unitOfWorkRepository.Main.DeleteMainAsync(id);

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
    #endregion
}

