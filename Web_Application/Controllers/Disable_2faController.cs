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
    public class Disable_2faController : Controller
    {
        string username;
        private readonly ConString _conString;
        private IHttpContextAccessor Accessor;
        private readonly IHtmlLocalizer<Disable_2faController> _localizer;


        public Disable_2faController(ConString conection,IHttpContextAccessor _accessor, IHtmlLocalizer<Disable_2faController> localizer)
        {
            _conString = conection;
            this.Accessor = _accessor;
            _localizer = localizer;
        }

        public IActionResult Disable_2fa()
        {
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            //Create a Cookie with a suitable Key and add the Cookie to Browser.
            Response.Cookies.Append("hide_layout", "true", option);
            //if log in hide unnecessary element
            ViewBag.hide_elements_layout = true;

            //Get data from localization and set
            var get_resource_data = _localizer["Text_confirme"];
            ViewData["Text_confirme"] = get_resource_data;

            get_resource_data = _localizer["Text_Introduce"];
            ViewData["Text_Introduce"] = get_resource_data;

            get_resource_data = _localizer["Text_pin"];
            ViewData["Text_pin"] = get_resource_data;

            get_resource_data = _localizer["Text_Manage"];
            ViewData["Text_Manage"] = get_resource_data;

            get_resource_data = _localizer["Text_Change"];
            ViewData["Text_Change"] = get_resource_data;

            get_resource_data = _localizer["Profile"];
            ViewData["Profile"] = get_resource_data;

            get_resource_data = _localizer["Mail"];
            ViewData["Mail"] = get_resource_data;

            get_resource_data = _localizer["Password"];
            ViewData["Password"] = get_resource_data;

            get_resource_data = _localizer["tfa"];
            ViewData["tfa"] = get_resource_data;

            get_resource_data = _localizer["Confirme"];
            ViewData["Confirme"] = get_resource_data;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disable_2fa(UserData user)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            Disable_2fa();
            
            var user_db = _conString.UserData.Single(userdata=>userdata.Username == username);
            
            if(user_db.Secretkey!=null)
            {
                string google_key = user_db.Secretkey;

                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

                string pin = user.TFA.Trim();

                bool status = tfa.ValidateTwoFactorPIN(google_key, pin, TimeSpan.FromSeconds(30));

                if (status == true)
                {
                    user_db.TFA = "0";
                    user_db.Secretkey = "0";
                    _conString.SaveChanges();   

                    ViewBag.display_2fa = "0";
                    Response.Cookies.Append("status_2fa", "0", option);

                    return RedirectToAction("TFA", "TFA"); }

                else
                    ViewData["TFAWarning"] = "Pin isn't corect";

                
            }
            return View();
        }

        [HttpPost]
        public ActionResult redirect_email()
        {           
            return RedirectToAction("Account_email", "Account_mail");
        }

        [HttpPost]
        public ActionResult redirect_password()
        {           
            return RedirectToAction("Account_password", "Account_pass");
        }

    }
}
