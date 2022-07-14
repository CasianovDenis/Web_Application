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
    public class TFAController : Controller
    {
        Guid random_secret_key = Guid.NewGuid();

        private IHttpContextAccessor Accessor;
        private readonly ConString _conString;
        private string username;
        private readonly IHtmlLocalizer<TFAController> _localizer;

        public TFAController(ConString conection,IHttpContextAccessor _accessor, IHtmlLocalizer<TFAController> localizer)
        {
            _conString = conection;
            this.Accessor = _accessor;
            _localizer = localizer;
        }

        public IActionResult TFA()
        {
            //extract data from cookie storage
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];
            ViewBag.display_2fa = this.Accessor.HttpContext.Request.Cookies["status_2fa"];

            ViewData["Username"] = username;

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            //query select EF
            var user_db = _conString.UserData.Single(userdata=>userdata.Username==username);
               if (user_db.Email!=null) ViewData["Email"] = user_db.Email;

            //Create a Cookie with a suitable Key and add the Cookie to Browser.
            Response.Cookies.Append("hide_layout", "true", option);
            //if log in hide unnecessary element
            ViewBag.hide_elements_layout = true;

       
                if (user_db.TFA == "0") { ViewBag.display_2fa = "0"; ViewData["2FA"] = "Disabled"; }
                else
                         if (user_db.TFA == "1") { ViewBag.display = "1"; ViewData["2FA"] = "Enabled"; }
           

            //Get text for language set
            var get_resource_data = _localizer["TextManage"];
            ViewData["TextManage"] = get_resource_data;

            get_resource_data = _localizer["TextChange"];
            ViewData["TextChange"] = get_resource_data;

            get_resource_data = _localizer["Profile"];
            ViewData["Profile"] = get_resource_data;

            get_resource_data = _localizer["Mail"];
            ViewData["Mail"] = get_resource_data;

            get_resource_data = _localizer["Password"];
            ViewData["Password"] = get_resource_data;

            get_resource_data = _localizer["tfa"];
            ViewData["tfa"] = get_resource_data;

            get_resource_data = _localizer["User_name"];
            ViewData["User_name"] = get_resource_data;

            get_resource_data = _localizer["text_manage_tfa"];
            ViewData["text_manage_tfa"] = get_resource_data;

            get_resource_data = _localizer["Enable_tfa"];
            ViewData["Enable_tfa"] = get_resource_data;

            get_resource_data = _localizer["Disable_tfa"];
            ViewData["Disable_tfa"] = get_resource_data;

            get_resource_data = _localizer["warning_security"];
            ViewData["warning_security"] = get_resource_data;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TFA(UserData user)
        {
            TFA();
            if (ViewBag.display_2fa == "0") return RedirectToAction("Enable_2fa", "Enable_2fa");
            else
            if (ViewBag.display_2fa == "1") return RedirectToAction("Disable_2fa", "Disable_2fa");
            else
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
