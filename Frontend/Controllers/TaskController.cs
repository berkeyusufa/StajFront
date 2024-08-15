using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApplication6.Models;
using System.Linq;

public class TaskController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public TaskController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var client = _clientFactory.CreateClient();

        // Tüm görevleri getir
        var taskResponse = await client.GetAsync("https://localhost:7209/api/Task/GetAllTasks");
        var tasks = new List<WebApplication6.Models.Task>();

        if (taskResponse.IsSuccessStatusCode)
        {
            var taskContent = await taskResponse.Content.ReadAsStringAsync();
            tasks = JsonConvert.DeserializeObject<List<WebApplication6.Models.Task>>(taskContent);
        }

        // Task nesnelerini TaskViewModel listesine dönüştür
        var taskViewModels = tasks.Select(task => new TaskViewModel
        {
            Id = task.Id,
            SelectedDriverId = task.DriverId,
            SelectedBusId = task.BusId,
            TaskDate = task.TaskDate,
            Driver = task.Driver,
            Bus = task.Bus
        }).ToList();

        return View(taskViewModels);
    }

    [HttpGet]
    public async Task<IActionResult> AssignTask()
    {
        var client = _clientFactory.CreateClient();

        // Tüm görevleri getir
        var taskResponse = await client.GetAsync("https://localhost:7209/api/Task/GetAllTasks");
        var tasks = new List<WebApplication6.Models.Task>();

        if (taskResponse.IsSuccessStatusCode)
        {
            var taskContent = await taskResponse.Content.ReadAsStringAsync();
            tasks = JsonConvert.DeserializeObject<List<WebApplication6.Models.Task>>(taskContent);
        }

        var driverResponse = await client.GetAsync("https://localhost:7209/api/Drivers/GetAllDrivers");
        var drivers = new List<Driver>();

        if (driverResponse.IsSuccessStatusCode)
        {
            var driverContent = await driverResponse.Content.ReadAsStringAsync();
            drivers = JsonConvert.DeserializeObject<List<Driver>>(driverContent);
        }

        
        var busResponse = await client.GetAsync("https://localhost:7209/api/Bus/GetAllBuses");
        var buses = new List<Bus>();

        if (busResponse.IsSuccessStatusCode)
        {
            var busContent = await busResponse.Content.ReadAsStringAsync();
            buses = JsonConvert.DeserializeObject<List<Bus>>(busContent);
        }

        // ViewModel oluştur
        var viewModel = new TaskViewModel
        {
            Tasks = tasks.Select(task => new TaskViewModel
            {
                Id = task.Id,
                SelectedDriverId = task.DriverId,
                SelectedBusId = task.BusId,
                TaskDate = task.TaskDate,
                Driver = task.Driver,
                Bus = task.Bus
            }).ToList(),
            Drivers = drivers,
            Buses = buses
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AssignTask(TaskViewModel viewModel)
    {

        {
            var client = _clientFactory.CreateClient();

            var task = new WebApplication6.Models.Task
            {
                DriverId = viewModel.SelectedDriverId,
                BusId = viewModel.SelectedBusId,
                TaskDate = viewModel.TaskDate
            };

            var response = await client.PostAsJsonAsync("https://localhost:7209/api/Task/AssignTask", task);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                ViewBag.Message = "An error occurred while creating the task.";
                ViewBag.ErrorDetail = responseBody;
            }
        }

        viewModel.Tasks = await GetTasksAsync();
        viewModel.Drivers = await GetDriversAsync();
        viewModel.Buses = await GetBusesAsync();

        return View(viewModel);
    }

    private async Task<List<TaskViewModel>> GetTasksAsync()
    {
        var client = _clientFactory.CreateClient();
        var taskResponse = await client.GetAsync("https://localhost:7209/api/Task/GetAllTasks");
        var tasks = new List<WebApplication6.Models.Task>();

        if (taskResponse.IsSuccessStatusCode)
        {
            var taskContent = await taskResponse.Content.ReadAsStringAsync();
            tasks = JsonConvert.DeserializeObject<List<WebApplication6.Models.Task>>(taskContent);
        }

        // Task nesnelerini TaskViewModel'e dönüştür
        var taskViewModels = tasks.Select(task => new TaskViewModel
        {
            Id = task.Id,
            SelectedDriverId = task.DriverId,
            SelectedBusId = task.BusId,
            TaskDate = task.TaskDate,
            Driver = task.Driver,
            Bus = task.Bus
        }).ToList();

        return taskViewModels;
    }

    private async Task<List<Driver>> GetDriversAsync()
    {
        var client = _clientFactory.CreateClient();
        var driverResponse = await client.GetAsync("https://localhost:7209/api/Drivers/GetAllDrivers");
        var drivers = new List<Driver>();

        if (driverResponse.IsSuccessStatusCode)
        {
            var driverContent = await driverResponse.Content.ReadAsStringAsync();
            drivers = JsonConvert.DeserializeObject<List<Driver>>(driverContent);
        }

        return drivers;
    }

    private async Task<List<Bus>> GetBusesAsync()
    {
        var client = _clientFactory.CreateClient();
        var busResponse = await client.GetAsync("https://localhost:7209/api/Bus/GetAllBuses");
        var buses = new List<Bus>();

        if (busResponse.IsSuccessStatusCode)
        {
            var busContent = await busResponse.Content.ReadAsStringAsync();
            buses = JsonConvert.DeserializeObject<List<Bus>>(busContent);
        }

        return buses;
    }
}
