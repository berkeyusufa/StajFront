using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication6.Models;  
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;
using System.Linq;
using X.PagedList.Extensions;

public class BusController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public BusController(IHttpClientFactory clientFactory)
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

        var buses = await GetBusesAsync();
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        var pagedBuses = buses.ToPagedList(pageNumber, pageSize);

        return View(pagedBuses);
    }

    public async Task<IActionResult> Details(int id)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        var bus = await GetBusByIdAsync(id);
        if (bus == null)
        {
            return NotFound();
        }

        return View(bus);
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
    public async Task<IActionResult> Create(Bus bus)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.PostAsJsonAsync("https://localhost:7209/api/Bus/InsertBus", bus);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        else
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            ViewBag.Message = "An error occurred while adding the bus.";
            ViewBag.ErrorDetail = responseBody; // Detaylı hata mesajını yakalama
            Console.WriteLine($"Error response: {responseBody}");
            return View(bus);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        var bus = await GetBusByIdAsync(id);
        if (bus == null)
        {
            return NotFound();
        }

        return View(bus);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Bus Bus)
    {
        if (id != Bus.Id)
        {
            return BadRequest();
        }

        var client = _clientFactory.CreateClient();
        var response = await client.PutAsJsonAsync($"https://localhost:7209/api/Bus/UpdateBus/{id}", Bus);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        else
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            ViewBag.Message = "An error occurred while updating the bus.";
            ViewBag.ErrorDetail = responseBody;
            return View(Bus);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        var bus = await GetBusByIdAsync(id);
        if (bus == null)
        {
            return NotFound();
        }

        return View(bus);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.DeleteAsync($"https://localhost:7209/api/Bus/DeleteBusById/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        else
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            ViewBag.Message = "An error occurred while deleting the bus.";
            ViewBag.ErrorDetail = responseBody;
            return RedirectToAction("Delete", new { id });
        }
    }

    private async Task<List<Bus>> GetBusesAsync()
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetAsync("https://localhost:7209/api/Bus/GetAllBuses");

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var buses = JsonConvert.DeserializeObject<List<Bus>>(responseBody);
            return buses.OrderByDescending(b => b.Id).ToList(); // Yeni eklenenleri en üste almak için sıralama
        }
        else
        {
            return new List<Bus>();
        }
    }

    private async Task<Bus> GetBusByIdAsync(int id)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetAsync($"https://localhost:7209/api/Bus/GetBusById/{id}");

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var bus = JsonConvert.DeserializeObject<Bus>(responseBody);
            return bus;
        }
        else
        {
            return null;
        }
    }
}
