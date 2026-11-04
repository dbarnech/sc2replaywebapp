using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sc2ReplayWebApp.Controllers;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        return Challenge(
            new AuthenticationProperties
            {
                RedirectUri = "/"
            },
            "BattleNet");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        Response.Cookies.Delete(".AspNetCore.Cookies");

        return RedirectToAction(
            "Index",
            "Home");
    }

    [Authorize]
    public IActionResult Profile()
    {


        return Json(
            User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            }));
    }
}
