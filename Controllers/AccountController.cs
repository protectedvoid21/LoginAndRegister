using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLoginTest.Models;

namespace WebLoginTest.Controllers {
    public class AccountController : Controller {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public ViewResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AppUser user) {
            if (!ModelState.IsValid) {
                return View();
            }

            if (await userManager.FindByEmailAsync(user.Email) == null) {
                IdentityResult result = await userManager.CreateAsync(user, user.PasswordHash);
                if(result.Succeeded) {
                    ViewBag.Message = $"Hello {user.FirstName}!";
                }
                else {
                    foreach(var error in result.Errors) {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View();
                }
            }
            else {
                ViewBag.Message = "User with the same email is already created";
            }

            return View("LoggedIn");
        }

        [HttpGet]
        public ViewResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel) {
            if(!ModelState.IsValid) {
                return View();
            }

            AppUser userByEmail = await userManager.FindByEmailAsync(loginModel.Email);

            var result = await signInManager.PasswordSignInAsync(userByEmail, loginModel.Password, false, false);
            Console.WriteLine(result);
            if(result.Succeeded) {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Message = "Login failed! Incorrect email or password.";
            return View();
        }

        public IActionResult Logout() {
            signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
