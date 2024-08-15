using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication6.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;
using System.Linq;
using X.PagedList.Extensions;

public class DriversController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public DriversController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> Index(int? page)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        var drivers = await GetDriversAsync();
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        var pagedDrivers = drivers.ToPagedList(pageNumber, pageSize);

        return View(pagedDrivers);
    }

    public async Task<IActionResult> Details(int id)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        var driver = await GetDriverByIdAsync(id);
        if (driver == null)
        {
            return NotFound();
        }

        return View(driver);
    }

    public IActionResult Create()
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Driver driver)
    {
        driver.BirthDate = CalculateBirthDate(driver.Age);
        driver.CreatedDate = DateTime.Today; // Sadece tarihi alır.

        var client = _clientFactory.CreateClient();
        var response = await client.PostAsJsonAsync("https://localhost:7209/api/Drivers/InsertDriver", driver);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        else
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            ViewBag.Message = "An error occurred while adding the driver.";
            ViewBag.ErrorDetail = responseBody; // Detaylı hata mesajını yakalama
            Console.WriteLine($"Error response: {responseBody}");
            return View(driver);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        var driver = await GetDriverByIdAsync(id);
        if (driver == null)
        {
            return NotFound();
        }

        return View(driver);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Driver driver)
    {
        if (id != driver.Id)
        {
            return BadRequest();
        }

        driver.BirthDate = CalculateBirthDate(driver.Age);

        var client = _clientFactory.CreateClient();
        var response = await client.PutAsJsonAsync($"https://localhost:7209/api/Drivers/UpdateDriver/{id}", driver);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        else
        {
            ViewBag.Message = "An error occurred while updating the driver.";
            return View(driver);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        var driver = await GetDriverByIdAsync(id);
        if (driver == null)
        {
            return NotFound();
        }

        return View(driver);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.DeleteAsync($"https://localhost:7209/api/Drivers/DeleteDriverById/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        else
        {
            ViewBag.Message = "An error occurred while deleting the driver.";
            return RedirectToAction("Delete", new { id });
        }
    }

    private async Task<List<Driver>> GetDriversAsync()
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetAsync("https://localhost:7209/api/Drivers/GetAllDrivers");

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var drivers = JsonConvert.DeserializeObject<List<Driver>>(responseBody);
            return drivers.OrderByDescending(d => d.Id).ToList(); // Yeni eklenenleri en üste almak için sıralama
        }
        else
        {
            return new List<Driver>();
        }
    }

    private async Task<Driver> GetDriverByIdAsync(int id)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetAsync($"https://localhost:7209/api/Drivers/GetDriverById/{id}");

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var driver = JsonConvert.DeserializeObject<Driver>(responseBody);
            return driver;
        }
        else
        {
            return null;
        }
    }

    private DateTime CalculateBirthDate(int age)
    {
        var today = DateTime.Today;
        var birthDate = today.AddYears(-age);
        if (birthDate > today)
        {
            birthDate = birthDate.AddYears(-1);
        }
        return birthDate;
    }
}
