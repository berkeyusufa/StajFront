using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication6.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public HomeController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public IActionResult Index()
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        return RedirectToAction("Index", "Drivers"); // Sürücü listesini gösteren sayfaya yönlendir
    }

    public async Task<IActionResult> GetAllDrivers()
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        var drivers = await GetDriversAsync();
        return View(drivers);
    }

    private async Task<List<Driver>> GetDriversAsync()
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetAsync("https://localhost:7209/api/Drivers/GetAllDrivers");

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var drivers = JsonConvert.DeserializeObject<List<Driver>>(responseBody);
            return drivers;
        }
        else
        {
            return new List<Driver>();
        }
    }
}
