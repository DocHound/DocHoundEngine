using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DocHound.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore;

namespace DocHound.Views.Account
{
    public class AccountController : BaseController
    {
        [HttpGet]
        [Route("account/signin")]
        public ActionResult Signin()
        {
            var model = CreateViewModel<SigninViewModel>();
            return View("SignIn", model);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("account/signin")]
        public async Task<ActionResult> SignIn(SigninViewModel model)
        {
            InitializeViewModel(model);

            if (!ModelState.IsValid)
            {
                model.ErrorDisplay.AddMessages(ModelState);
                model.ErrorDisplay.ShowError("Please correct the following:");
                return View(model);
            }

            var userBus = new UserBusiness();
            var user = await userBus.Authenticate(model.Email, model.Password);
            if (user == null)
            {
                model.ErrorDisplay.ShowError(userBus.ErrorMessage);
                return View(model);
            }

            var identity = AppUser.GetClaimsIdentityFromUser(user);


            // Set cookie and attach claims
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity), new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(2)
                });


            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return Redirect(model.ReturnUrl);



            return Redirect("~/");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("~/");
        }

    }

    public class SigninViewModel : BaseViewModel
    {
        internal User User { get; set; }

        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberPassword { get; set; }

        public string ReturnUrl { get; set; }
    }

}