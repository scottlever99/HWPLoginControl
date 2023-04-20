using HWPLoginControl.Enums;
using HWPLoginControl.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace HWPLoginControl.Controllers
{
    public class ActivateController : Controller
    {
        private readonly Services.AccountService _accountService;

        public ActivateController(Services.AccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Account([FromRoute]int id)
        {
            if (id < 1) return BadRequest();

            if (await _accountService.UserEnabled(id)) return BadRequest();
            //if user is enabled - but wants to renew payment?

            return View(new ActivateModel(id)); 
        }

        public IActionResult Promo([FromRoute]int id)
        {
            //Again check if activated

            return View();
        }

        public IActionResult Payment([FromRoute]int id)
        {
            //Again check if activated

            return Redirect("https://buy.stripe.com/test_cN215R75ogOagYo4gh?client_reference_id=" + id.ToString());
        }

        public async Task<IActionResult> PaymentRedirect([FromQuery]string checkout_session_id)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51MygchAlSoLxBcHKjc6T16QCWcuHrZTOeJ2l67Fr4EbdbSLpwtzEXTpV6mAp0DkMnQV6G8o2SbVjZYF3VwtgIVFI00yjhgK614";
                var service = new SessionService();
                var session = service.Get(checkout_session_id);
                //var session = service.Get("cs_test_b12lBj0nn01sbiWp5eWBGseLbgMPvDtE1bSSuKVtZKOlINBSMIpiqvWC3t");

                var userId = session.ClientReferenceId;
                if (string.IsNullOrEmpty(userId)) return RedirectToAction("Activated", new { success = false, firstname = "" });
                var user = await _accountService.GetUser(Convert.ToInt32(userId));
                if (user == null) return RedirectToAction("Activated", new { success = false, firstname = "" });

                var result = await _accountService.EnableUser(user.id, (int)Gateway.Stripe);

                return RedirectToAction("Activated", new { success = result, firstname = user.firstname });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Activated", new { success = false, firstname = "" });
            }
        }

        public IActionResult Activated([FromQuery]bool success, [FromQuery]string? firstname)
        {
            ViewBag.Success = success;
            ViewBag.Name = "Customer";
            if (!string.IsNullOrEmpty(firstname)) ViewBag.Name = firstname;
            return View();
        }
    }
}
