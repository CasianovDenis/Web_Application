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
    public class VerificationController : Controller
    {
        private readonly IHtmlLocalizer<VerificationController> _localizer;
        private readonly ConString _conString;
        private IHttpContextAccessor Accessor;
        private string username;

        public VerificationController(ConString conection, IHttpContextAccessor _accessor, IHtmlLocalizer<VerificationController> localizer)
        {
            this.Accessor = _accessor;
            _localizer = localizer;
            _conString = conection;
        }

        public ActionResult Verification_email()
        {
            //extract data from cookie storage
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];
            //set form display
            ViewBag.display_2fa = this.Accessor.HttpContext.Request.Cookies["status_2fa"];

            ViewData["Username"] = username;

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);       

            //Query select EF
            var user_db = _conString.UserData.Single(userdata=>userdata.Username==username);
            if (user_db.Email!=null)
            {
                ViewData["Email"] = user_db.Email;
                ViewBag.email = user_db.Email;
            }

            //Create a Cookie with a suitable Key and add the Cookie to Browser.
            Response.Cookies.Append("hide_layout", "true", option);
            //if log in hide unnecessary element
            ViewBag.hide_elements_layout = true;


            //Get data from localization and set
            var get_resource_data = _localizer["Text_confirme"];
            ViewData["Text_confirme"] = get_resource_data;        

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

        public ActionResult Verification_password()
        {
            //extract data from cookie storage
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];
            //set form display

            ViewData["Username"] = username;

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);


            //Query select EF
            var user_db = _conString.UserData.Single(userdata => userdata.Username == username);
            if (user_db.Email != null)
            {
                ViewData["Email"] = user_db.Email;
                ViewBag.email = user_db.Email;
            }


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
        public ActionResult Verification_email(UserData user)
        {
            Verification_email();

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);



            //check key
            string key_email = this.Accessor.HttpContext.Request.Cookies["secret_key_email"];

            if (user.Secretkey == key_email)
            {             
                //Query update EF
                var user_db = _conString.UserData.Single(userdata => userdata.Username == username);
                user_db.Email =  this.Accessor.HttpContext.Request.Cookies["new_email"];
                _conString.SaveChanges();
             
                return RedirectToAction("Account_email", "Account_mail");
            }
            else
                ViewData["WarningVerification"] = "Key does not match";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verification_password(UserData user)
        {
            Verification_password();

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);


            string key_password = this.Accessor.HttpContext.Request.Cookies["secret_key_password"];
            string password = this.Accessor.HttpContext.Request.Cookies["password"];
            user.Password = password;

            
                if (user.Secretkey == key_password)
                {            
                //Query update EF
                var user_db = _conString.UserData.Single(userdata => userdata.Username == username);
                user_db.Password = this.Accessor.HttpContext.Request.Cookies["password"];
                _conString.SaveChanges();
                                                 
                    return RedirectToAction("Account_password", "Account_pass");
                }
                else
                    
                return View();           
        }

        [HttpPost]
        public ActionResult redirect_on_2fa()
        {
            return RedirectToAction("TFA", "TFA");
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
