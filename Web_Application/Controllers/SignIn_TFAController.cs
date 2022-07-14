using Google.Authenticator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Web_Application.Models;

namespace Web_Application.Controllers
{
    public class SignIn_TFAController : Controller
    {
        string username;
        private readonly ConString _conString;
        private IHttpContextAccessor Accessor;
        private readonly IHtmlLocalizer<SignIn_TFAController> _localizer;


        public SignIn_TFAController(ConString conection, IHttpContextAccessor _accessor, IHtmlLocalizer<SignIn_TFAController> localizer)
        {
            _conString = conection;
            this.Accessor = _accessor;
            _localizer = localizer;
        }

        public IActionResult TFA_LogIn()
        {
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];

            //Get data from localization and set
            var get_resource_data = _localizer["Text_confirme"];
            ViewData["Text_confirme"] = get_resource_data;

            get_resource_data = _localizer["Text_Introduce"];
            ViewData["Text_Introduce"] = get_resource_data;

            get_resource_data = _localizer["Text_pin"];
            ViewData["Text_pin"] = get_resource_data;

            get_resource_data = _localizer["SignIn"];
            ViewData["SignIn"] = get_resource_data;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TFA_LogIn(UserData user)
        {
            TFA_LogIn();
            //Query select EF
            var user_db = _conString.UserData.Single(userdata=>userdata.Username == username);

            if (user_db.Secretkey!=null)
            {
                string google_key = user_db.Secretkey;

                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

                string pin = user.TFA.Trim();

                bool status = tfa.ValidateTwoFactorPIN(google_key, pin, TimeSpan.FromSeconds(30));

                if (status == true) return RedirectToAction("Account", "Account");

                else
                {
                    var get_resource_data = _localizer["Incorectpin"];
                    ViewData["Incorectpin"] = get_resource_data;
                }             
            }          
            return View();
        }
    }
}
