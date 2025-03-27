using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TirtaRK.Models;
using System.Threading.Tasks;
using TirtaRK.Interfaces;

namespace TirtaRK.Controllers;

public class ArticleController : Controller
{
    
    private readonly ILogger<ArticleController> _logger;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public ArticleController( IUnitOfWorkRepository unitOfWorkRepository)
    {
        this._unitOfWorkRepository = unitOfWorkRepository;
    }

    public async Task<IActionResult> Index()
    {
        
        var model = await _unitOfWorkRepository.Article.GetListArticleAsync();
        return View(model);
    }
    
    [Route("/article/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var model = await _unitOfWorkRepository.Article.GetArticleBySlugAsync(slug);

        if (model == null)
        {
            return View("~/Views/404/PageNotFound.cshtml");
        }
        return View(model);
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
