using FerticaStockManagement.Models;
using FerticaStockManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FerticaStockManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ========================================
        // LOGIN - GET
        // ========================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        // ========================================
        // LOGIN - POST
        // ========================================

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction(
                    "Index",
                    "Dashboard"
                );
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(
                    "",
                    "Votre compte est temporairement bloqué."
                );
            }
            else
            {
                ModelState.AddModelError(
                    "",
                    "Email ou mot de passe incorrect."
                );
            }

            return View(model);
        }

        // ========================================
        // REGISTER - GET
        // ========================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // ========================================
        // REGISTER - POST
        // ========================================

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(
                user,
                model.Password
            );

            if (result.Succeeded)
            {
                // EVERY NORMAL REGISTRATION = EMPLOYEE
                await _userManager.AddToRoleAsync(
                    user,
                    "Employee"
                );

                await _signInManager.SignInAsync(
                    user,
                    isPersistent: false
                );

                return RedirectToAction(
                    "Index",
                    "Dashboard"
                );
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    "",
                    error.Description
                );
            }

            return View(model);
        }

        // ========================================
        // LOGOUT
        // ========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(
                "Login",
                "Account"
            );
        }

        // ========================================
        // ACCESS DENIED
        // ========================================

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ========================================
        // PROFILE
        // ========================================

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }
    
    // ========================================
// CHANGE PASSWORD - GET
// ========================================

[HttpGet]
[Authorize]
public IActionResult ChangePassword()
{
    return View();
}

// ========================================
// CHANGE PASSWORD - POST
// ========================================

[HttpPost]
[Authorize]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ChangePassword(
    ChangePasswordViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    var user = await _userManager.GetUserAsync(User);

    if (user == null)
    {
        return RedirectToAction("Login");
    }

    var result = await _userManager.ChangePasswordAsync(
        user,
        model.CurrentPassword,
        model.NewPassword
    );

    if (result.Succeeded)
    {
        await _signInManager.RefreshSignInAsync(user);

        TempData["SuccessMessage"] =
            "Votre mot de passe a été modifié avec succès.";

        return RedirectToAction("Profile");
    }

    foreach (var error in result.Errors)
    {
        ModelState.AddModelError(
            "",
            error.Description
        );
    }

    return View(model);
}
}
}
