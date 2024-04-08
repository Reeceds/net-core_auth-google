using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Admin;

[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult GoogleLogin()
    {
        string redirectUrl = Url.Action("GoogleResponse", "Account")!;
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        return new ChallengeResult("Google", properties);
    }

    [AllowAnonymous]
    public async Task<IActionResult> GoogleResponse()
    {
        ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();

        if (info == null) return RedirectToAction("Index", "Home");

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
        string[] userInfo = { info.Principal.FindFirst(ClaimTypes.Name)!.Value, info.Principal.FindFirst(ClaimTypes.Email)!.Value };

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            AppUser user = new AppUser
            {
                Email = info.Principal.FindFirst(ClaimTypes.Email)!.Value,
                UserName = info.Principal.FindFirst(ClaimTypes.Email)!.Value
            };

            IdentityResult identResult = await _userManager.CreateAsync(user);

            if (identResult.Succeeded)
            {
                identResult = await _userManager.AddLoginAsync(user, info);
                if (identResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
            }

            return AccessDenied();
        }
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

