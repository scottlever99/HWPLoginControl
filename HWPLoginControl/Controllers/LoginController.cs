using HWPLoginControl.Models;
using HWPLoginControl.Services;
using Microsoft.AspNetCore.Mvc;

namespace HWPLoginControl.Controllers
{
    public class LoginController : Controller
    {
        private readonly AccountService _accountService;

        public LoginController(AccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Create");
        }

        public IActionResult Create()
        {
            ViewBag.PasswordInvalid = false;
            ViewBag.UserAlreadyExists = false;
            ViewBag.CreationFailed = false;

            return View(new CreateAccount());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccount model)
        {
            ViewBag.PasswordInvalid = false;
            ViewBag.UserAlreadyExists = false;
            ViewBag.CreationFailed = false;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.Password.Any(char.IsDigit) || !model.Password.Any(char.IsLetter) || !model.Password.Any(char.IsUpper) || !model.Password.Any(char.IsLower))
            {
                ViewBag.PasswordInvalid = true;
                return View(model);
            }

            if (await _accountService.UserAlreadyExists(model.Email))
            {
                ViewBag.UserAlreadyExists = true;
                return View(model);
            }

            if (!await _accountService.CreateAccount(model))
            {
                ViewBag.CreationFailed = true;
                return View(model);
            }

            return RedirectToAction("Created");
        }

        public IActionResult Created()
        {
            return View();
        }

        public IActionResult ForgottenPassword(string? token)
        {
            ViewBag.Failed = false;
            ViewBag.PasswordInvalid = false;

            if (String.IsNullOrEmpty(token)) return NotFound();

            var email = "22@22.22";
            var tempEmail = _accountService.GetEmailFromString(token);
            if (String.IsNullOrEmpty(tempEmail)) return NotFound();
            if (_accountService.IsValidEmail(tempEmail)) email = tempEmail;

            var model = new ForgottenPassword();
            model.Email = email;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ForgottenPassword(ForgottenPassword model)
        {
            ViewBag.Failed = false;
            ViewBag.PasswordInvalid = false;

            if (!ModelState.IsValid) return View(model);

            if (!model.Password.Any(char.IsDigit) || !model.Password.Any(char.IsLetter) || !model.Password.Any(char.IsUpper) || !model.Password.Any(char.IsLower))
            {
                ViewBag.PasswordInvalid = true;
                return View(model);
            }

            if (!await _accountService.UpdatePassword(model.Email, model.Password))
            {
                ViewBag.Failed = true;
                return View(model);
            }

            return RedirectToAction("Created");
        }

        public IActionResult ForgottenPasswordEmail()
        {
            ViewBag.InvalidEmail = false;
            ViewBag.TempLink = "";
            ViewBag.Sent = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgottenPasswordEmail(ForgottenPasswordEmail model)
        {
            ViewBag.InvalidEmail = false;
            ViewBag.TempLink = "";
            ViewBag.Sent = false;

            if (!ModelState.IsValid) return View(model);

            //if (!await _accountService.UserAlreadyExists(model.Email))
            //{
            //    ViewBag.InvalidEmail = true;
            //    return View(model);
            //}

            var urlstring = _accountService.GetForgottenPasswordToken(model.Email);
            ViewBag.TempLink = urlstring;
            ViewBag.Sent = true;

            return View(model);
        }
    }
}
