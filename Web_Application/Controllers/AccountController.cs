using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Net;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Localization;

namespace Web_Application.Controllers
{
    public class AccountController : Controller
    {      
        Guid random_secret_key = Guid.NewGuid();

        private IHttpContextAccessor Accessor;
        
        private string username;
        private readonly ConString _conString;
        private readonly IHtmlLocalizer<AccountController> _localizer;


        public AccountController(ConString conection,IHttpContextAccessor _accessor, IHtmlLocalizer<AccountController> localizer)
        {
            _conString = conection;
            this.Accessor = _accessor;
            _localizer = localizer;
        }

        public IActionResult Account()
        {
          //extract data from cookie storage
             username = this.Accessor.HttpContext.Request.Cookies["UserName"];

            ViewData["Username"] = username;

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

           
            //get email fron table
            
            var user_db = _conString.UserData.Single(userdata => userdata.Username == username);
           
              ViewData["Email"] = user_db.Email;
            if (user_db.TFA=="0") { ViewBag.display_2fa = "0"; Response.Cookies.Append("status_2fa", "0", option); }
                else
                         if (user_db.TFA == "1") { ViewBag.display_2fa = "1"; Response.Cookies.Append("status_2fa", "1", option); }
                       

            //Create a Cookie with a suitable Key and add the Cookie to Browser.
            Response.Cookies.Append("hide_layout", "true", option);
            //if log in hide unnecessary element
            ViewBag.hide_elements_layout = true;


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

            get_resource_data = _localizer["warning_security"];
            ViewData["warning_security"] = get_resource_data;
            return View();
        }


       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Account(UserData user)
        {
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

        [HttpPost]
        public ActionResult redirect_on_2fa()
        {           

            return RedirectToAction("TFA", "TFA");

        }
    } 
}
