using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebApplication6.Models;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public AccountController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginModel user)
    {
        var client = _clientFactory.CreateClient();
        var json = JsonConvert.SerializeObject(user);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://localhost:7209/api/Users/Login", content);

        if (response.IsSuccessStatusCode)
        {
            HttpContext.Session.SetString("Username", user.Username);
            return RedirectToAction("Index", "Drivers");
        }
        else
        {
            ViewBag.Message = "Kullanıcı adı veya şifre yanlış.";
            return View();
        }
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login");
        }

        model.Username = username;

        var client = _clientFactory.CreateClient();
        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://localhost:7209/api/Users/ChangePassword", content);

        if (response.IsSuccessStatusCode)
        {
            ViewBag.Message = "Parola başarıyla değiştirildi.";
        }
        else
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            ViewBag.Message = "Mevcut parola yanlış.";
            ViewBag.ErrorDetail = responseBody; // Detaylı hata mesajını yakalama
        }

        return View();
    }
}

public class UserLoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class ChangePasswordModel
{
    public string Username { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}