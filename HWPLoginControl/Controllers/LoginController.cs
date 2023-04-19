using Azure;
using Azure.Communication.Email;
using HWPLoginControl.Models;
using HWPLoginControl.Services;
using Microsoft.AspNetCore.Mvc;

namespace HWPLoginControl.Controllers
{
    public class LoginController : Controller
    {
        private readonly AccountService _accountService;
        private readonly EmailService _emailService;

        public LoginController(AccountService accountService, EmailService emailService)
        {
            _accountService = accountService;
            _emailService = emailService;
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
            ViewBag.Success = false;

            return View(new CreateAccount());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccount model)
        {
            ViewBag.PasswordInvalid = false;
            ViewBag.UserAlreadyExists = false;
            ViewBag.CreationFailed = false;
            ViewBag.Success = false;

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

            ViewBag.Success = true;

            return View(new CreateAccount());
            //return RedirectToAction("Created");
        }

        public IActionResult Created()
        {
            return View();
        }

        public IActionResult ForgottenPassword(string? token)
        {
            ViewBag.Failed = false;
            ViewBag.PasswordInvalid = false;
            ViewBag.Success = false;

            if (String.IsNullOrEmpty(token)) return NotFound();

            var email = "fakeemail@fakeemail.fakeemail";
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
            ViewBag.Success = false;

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

            var returnModel = new ForgottenPassword();
            returnModel.Email = "fakeemail@fakeemail.fakeemail";
            ViewBag.Success = true;
            return View(returnModel);
        }

        public IActionResult ForgottenPasswordEmail()
        {
            ViewBag.Sent = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgottenPasswordEmail(ForgottenPasswordEmail model)
        {
            ViewBag.Sent = false;

            if (!ModelState.IsValid) return View(model);

            if (!await _accountService.UserAlreadyExists(model.Email))
            {
                ViewBag.Sent = true;
                return View(model);
            }

#if DEBUG
            var link = "https://localhost:7293/Login/ForgottenPassword?token=";

#else
            var link = "https://hwplogin.azurewebsites.net/Login/ForgottenPassword?token=";

#endif

            var urlstring = _accountService.GetForgottenPasswordToken(model.Email);
            link += urlstring;

            _emailService.SendForgottenPasswordEmail(model.Email, link);

            ViewBag.Sent = true;
            return View(model);
        }
    }
}
