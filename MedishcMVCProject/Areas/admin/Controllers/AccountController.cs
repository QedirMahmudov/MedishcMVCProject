using MedishcMVCProject.Models;
using MedishcMVCProject.Utilities.Enums;
using MedishcMVCProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.Areas.admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            ViewBag.Roles = Enum.GetValues(typeof(UserRole))
                .Cast<UserRole>()
                .Select(r => new SelectListItem
                {
                    Text = r.ToString(),
                    Value = r.ToString()
                }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            ViewBag.Roles = Enum.GetValues(typeof(UserRole))
                .Cast<UserRole>()
                .Select(r => new SelectListItem
                {
                    Text = r.ToString(),
                    Value = r.ToString()
                }).ToList();

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
            await _userManager.AddToRoleAsync(user, registerVM.Role);
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Login(LoginVM loginVM, string? returnUrl)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginVM.UserNameOrEmail || u.Email == loginVM.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Username,Email or password is incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.IsPersistent, true);



            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account Blocked! Wait 10 minutes!");
                return View();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Username,Email or password is incorrect");
                return View();

            }

            if (returnUrl is null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }


            return Redirect(returnUrl);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");


        }


        //public async Task<IActionResult> CreateRoles()
        //{
        //    foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
        //    {
        //        if (!await _roleManager.RoleExistsAsync(role.ToString()))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole
        //            {
        //                Name = role.ToString()
        //            });
        //        }
        //    }
        //    return RedirectToAction(nameof(HomeController.Index), "Home");
        //}
    }
}