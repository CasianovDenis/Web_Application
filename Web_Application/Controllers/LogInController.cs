using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Web_Application.Models;
using static System.Text.Encodings.Web.HtmlEncoder;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Web_Application.Controllers
{
    public class LogInController : Controller
    {

        private IHttpContextAccessor Accessor;
        private readonly ConString _conString;
        private readonly IHtmlLocalizer<LogInController> _localizer;

        public LogInController(ConString conection,IHttpContextAccessor _accessor, IHtmlLocalizer<LogInController> localizer)
        {
            _conString = conection;
            this.Accessor = _accessor;
            _localizer = localizer;
        }

        public IActionResult SignIn()
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            //Create a Cookie with a suitable Key and add the Cookie to Browser.
            Response.Cookies.Append("hide_layout", "false", option);

            ViewBag.hide_elements_layout = false;
            ViewBag.statusbox= this.Accessor.HttpContext.Request.Cookies["statusbox"];
            string username = this.Accessor.HttpContext.Request.Cookies["UserName"];
            string statusbox = this.Accessor.HttpContext.Request.Cookies["statusbox"];

            var user_db = _conString.UserData.Single(userdata => userdata.Username == username);

            if (statusbox == "true")
            {

                ViewBag.save_username = user_db.Username;
                ViewBag.save_password = user_db.Password;
            }
            else
                    if (statusbox == "false")
            {
                ViewBag.save_username = "";
                ViewBag.save_username = "";
            }

            //Get text for language set
            var get_resource_data = _localizer["User_name"];
            ViewData["User_name"] = get_resource_data;

            get_resource_data = _localizer["Password"];
            ViewData["Password"] = get_resource_data;

            get_resource_data = _localizer["SignIn"];
            ViewData["SignIn"] = get_resource_data;

            get_resource_data = _localizer["Remember"];
            ViewData["Remember"] = get_resource_data;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(UserData user)
        {

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);
          
            //create select query to table UserData use Entity Framework
            var user_db = _conString.UserData.Single(userdata => userdata.Username == user.Username);

            if (user_db.TFA == "1")
            {
                if (user_db.Username == user.Username)
                {
                    if (user_db.Password == user.Password)
                    {
                        Response.Cookies.Append("UserName", user.Username, option);

                        return RedirectToAction("TFA_LogIn", "SignIn_TFA");
                    }
                }
            }
            else
                if (user_db.TFA == "0")
            {

                if (user_db.Username == user.Username)
                {

                    if (user_db.Password == user.Password)
                    {

                        Response.Cookies.Append("UserName", user.Username, option);

                        return RedirectToAction("Account", "Account");
                    }
                }
            }

            var get_resource_data = _localizer["Incorectdata"];
            ViewData["Incorectdata"] = get_resource_data;
                       
            return View();
            }

        [HttpPost]
        public ActionResult checkbox()
        {
            string statusbox = this.Accessor.HttpContext.Request.Cookies["statusbox"];
            
            if (statusbox=="true") {
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(1);

                //Create a Cookie with a suitable Key and add the Cookie to Browser.
                Response.Cookies.Append("statusbox", "false", option);

                ViewBag.statusbox = "false";
            }
            else
                    if (statusbox=="false")
            {
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(1);

                //Create a Cookie with a suitable Key and add the Cookie to Browser.
                Response.Cookies.Append("statusbox", "true", option);

                ViewBag.statusbox = "true";
            }

             return RedirectToAction("SignIn", "LogIn");
        }

        }
    }

