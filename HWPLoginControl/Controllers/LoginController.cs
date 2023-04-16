using HWPLoginControl.Models;
using HWPLoginControl.Service;
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

            if (await _accountService.UserAlreadyExists(model))
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

        public ActionResult Created()
        {
            return View();
        }

        public IActionResult ForgottenPassword()
        {
            return View();
        }
    }
}
