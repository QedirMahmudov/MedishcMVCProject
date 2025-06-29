using MedishcMVCProject.Models;
using MedishcMVCProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class PharmacyAccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public PharmacyAccountController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM registerVM)
    {
        if (!ModelState.IsValid) return View(registerVM);

        AppUser user = new AppUser
        {
            Name = registerVM.Name,
            Surname = registerVM.Surname,
            UserName = registerVM.UserName,
            Email = registerVM.Email
        };

        IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(registerVM);
        }

        await _userManager.AddToRoleAsync(user, "Pharmacy");
        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToAction("Index", "Shop");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM loginVM, string? returnUrl)
    {
        if (!ModelState.IsValid) return View();

        AppUser user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.UserName == loginVM.UserNameOrEmail || u.Email == loginVM.UserNameOrEmail);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or email.");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.IsPersistent, true);

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Account is locked. Try again later.");
            return View();
        }

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials.");
            return View();
        }

        if (await _userManager.IsInRoleAsync(user, "Pharmacy"))
        {
            return RedirectToAction("Profile", "PharmacyAccount", new { area = "" });
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Shop");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Shop");
    }

    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        return View(user);
    }
}