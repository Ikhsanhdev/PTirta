using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TirtaRK.Models;
using TirtaRK.ViewModels;
using TirtaRK.Interfaces;
using TirtaRK.Models.Datatables;
using Serilog;
using TirtaRK.Repositories;
using System.Net.Http.Headers;

namespace TirtaRK.Controllers;

public class MainController : Controller
{
    private readonly ILogger<MainController> _logger;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IActivitiesRepository _activitiesRepository;
    private readonly IArticleRepository _articleRepository;

    public MainController(ILogger<MainController> logger, IUnitOfWorkRepository unitOfWorkRepository, IProjectRepository projectRepository, 
    IActivitiesRepository activitiesRepository, IArticleRepository articleRepository)
    {
        this._unitOfWorkRepository = unitOfWorkRepository;
        this._projectRepository = projectRepository;
        this._activitiesRepository = activitiesRepository;
        this._articleRepository = articleRepository;
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

    public IActionResult MapGoogle()
    {
        return View();
    }

    public async Task<IActionResult> Index()
    {
        // return View(new MainViewModel());
        try
        {
            var mains = await _unitOfWorkRepository.Main.GetAllAsync();
            var works = await _unitOfWorkRepository.Work.GetListWorkAsync();
            var gallery = await _unitOfWorkRepository.Gallery.GetListGalleryAsync();
            var services = await _unitOfWorkRepository.Service.GetListServiceAsync();
            // var projects = await _projectRepository.GetListProjectAsync();
            // var activity = await _activitiesRepository.GetListActivityAsync();
            // var article = await _articleRepository.GetListArticleAsync();

            if (mains == null || !mains.Any())
            {
                _logger.LogWarning("No data found");
                return View(new MainViewModel());
            }

            var viewModel = new MainViewModel
            {
                Posters = mains.Where(m => m.Category == "poster").ToList(),
                Tombol = mains.Where(m => m.Category == "tombol").ToList(),
                Layanan = mains.Where(m => m.Category == "layanan").ToList(),
                About = mains.FirstOrDefault(m => m.Category == "about"),
                Profile = mains.FirstOrDefault(m => m.Category == "profile"),
                Testimoni = mains.FirstOrDefault(m => m.Category == "testimonial"),
                FAQ = mains.Where(m => m.Category == "faq").ToList(),
                Klien = mains.Where(m => m.Category == "klien").ToList(),
                Vision = mains.Where(m => m.Category == "visi").OrderByDescending(d => d.CreatedAt).Take(2).ToList(),
                Mission = mains.Where(m => m.Category == "misi").OrderByDescending(d => d.CreatedAt).Take(2).ToList(),
                
                Works = works.OrderByDescending(w => w.CreatedAt).Take(4).ToList(),
                Galleries = gallery.OrderByDescending(g => g.CreatedAt).Take(6).ToList(),
                Services = services.OrderByDescending(s => s.CreatedAt).Take(6).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading data: {Message}", ex.Message);
            return View(new MainViewModel());
        }
    }

    private async Task<dynamic> GetDataApi(string endPoint){
        
        string apiUrl = $"http://localhost:5000/{endPoint}"; 
        string username = "m0n1tor_st4tion";
        string password = "H1gertech.1dua3";

        using (HttpClient client = new HttpClient())
        {
            // Set up basic authentication credentials
            string authHeaderValue = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    // Deserialize the response data to a JSON string
                    string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(responseData);
                    return responseData; // Output the JSON string
                }
                else
                {
                    return response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return ex.Message;
            }
        }
    }

    public async Task<JsonResult> GetStationAll(){
        string endPoint = $"LastReading/all/";
        var data = await GetDataApi(endPoint);
        return Json(data);
    }
}
