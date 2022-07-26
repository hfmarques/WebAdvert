﻿using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Accounts;

namespace Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> signInManager;
        private readonly UserManager<CognitoUser> userManager;
        private readonly CognitoUserPool pool;

        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager,
            CognitoUserPool pool)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.pool = pool;
        }

        [HttpGet]
        public IActionResult Signup()
        {
            var model = new SignupModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            if (ModelState.IsValid)
            {
                var user = pool.GetUser(model.Email);
                if (user.Status is not null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists.");
                    return View(model);
                }

                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
                var createdUser = await userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

                if (createdUser.Succeeded)
                    return RedirectToAction("Confirm");

                foreach (var item in createdUser.Errors) ModelState.AddModelError(item.Code, item.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Confirm(ConfirmModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> ConfirmPost(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

                if (user is null)
                {
                    ModelState.AddModelError("NotFound", "A user with the given email address was not found");
                    return View(model);
                }

                var result = await ((CognitoUserManager<CognitoUser>) userManager)
                    .ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);
                if (result.Succeeded) return RedirectToAction("Index", "Home");

                foreach (var item in result.Errors) ModelState.AddModelError(item.Code, item.Description);

                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(LoginModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPost(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false)
                        .ConfigureAwait(false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("LoginError", "Email and password do no match");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("ResetPassword")]
        public async Task<IActionResult> ResetPasswordPost(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

                if (user is null)
                {
                    ModelState.AddModelError("NotFound", "A user with the given email address was not found");
                    return View(model);
                }

                await user.ForgotPasswordAsync().ConfigureAwait(false);
            }

            return RedirectToAction("NewPassword", "Accounts");
        }

        [HttpGet]
        public IActionResult NewPassword(NewPasswordModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("NewPassword")]
        public async Task<IActionResult> NewPasswordPost(NewPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

                if (user is null)
                {
                    ModelState.AddModelError("NotFound", "A user with the given email address was not found");
                    return View(model);
                }

                await user.ConfirmForgotPasswordAsync(model.Token, model.NewPassword).ConfigureAwait(false);
            }

            return RedirectToAction("Login", "Accounts");
        }
    }
}