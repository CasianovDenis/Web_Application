using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Web_Application.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Web_Application.Controllers
{
    public class Account_passController : Controller
    {
        
        Guid random_secret_key = Guid.NewGuid();

        private IHttpContextAccessor Accessor;

        private string username;
        private readonly ConString _conString;
        private readonly IHtmlLocalizer<Account_passController> _localizer;

        public Account_passController(ConString conection, IHttpContextAccessor _accessor, IHtmlLocalizer<Account_passController> localizer)
        {
            _conString = conection;
            this.Accessor = _accessor;
            _localizer = localizer;

        }

        public IActionResult Account_password()
        {
           
            //extract data from cookie storage
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];
            ViewBag.display_2fa = this.Accessor.HttpContext.Request.Cookies["status_2fa"];


            ViewData["Username"] = username;

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            //create select query to table UserData use Entity Framework
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

            get_resource_data = _localizer["new_pass"];
            ViewData["new_pass"] = get_resource_data;

            get_resource_data = _localizer["Change_pass"];
            ViewData["Change_pass"] = get_resource_data;

            get_resource_data = _localizer["warning_security"];
            ViewData["warning_security"] = get_resource_data;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Account_password(UserData user)
        {
      
            //if load page to call constructor page
            Account_password();

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);
            

            //check if setting->password                    
                if (oldpass(user) == false)
                {
                    if (checkpass(user) == false)
                    {
                        Response.Cookies.Append("open_form", "verification", option);
                        //ViewBag.open_form = this.Accessor.HttpContext.Request.Cookies["open_form"];

                        SmtpClient Smtp = new SmtpClient("smtp.mail.ru", 587);
                        Smtp.EnableSsl = true;
                        Smtp.Credentials = new NetworkCredential("email@mail.ru", "password");//real email and password
                                                                                                                  //was hide

                        MailMessage Message = new MailMessage();
                        Message.From = new MailAddress("email@mail.ru");//real email was hide

                        Message.To.Add(new MailAddress(ViewBag.email));
                        Message.Subject = "Change password";
                        Message.Body = "Confirme if is you request to change password ,insert this code " + random_secret_key +
                            " in application window." + "\n" +
                            "If is not you request,please change password in application.";

                        Smtp.Send(Message);

                        Response.Cookies.Append("secret_key_password", random_secret_key.ToString(), option);
                       
                       
                        ViewData["WarningPassword"] = "Check you email";
                        return RedirectToAction("Verification_password", "Verification");
                    }
                    else
                        ViewData["WarningPassword"] = "Password is not secure";

                }
            

            return View("Account_password");
        }

        [HttpPost]
        public ActionResult redirect_email()
        {

            return RedirectToAction("Account_email", "Account_mail");

        }

        [HttpPost]
        public ActionResult redirect_on_2fa()
        {

            return RedirectToAction("TFA", "TFA");

        }

        public bool checkpass(UserData user)
        {
            bool flag = false;
            int count = 0;

            try
            {
                for (int index = 0; index < user.Password.Length; index++)
                {
                    if (Char.IsLetter(user.Password[index]) == true) count = count + 1;
                    if (Char.IsNumber(user.Password[index]) == true) count = count + 1;
                    if (Char.IsUpper(user.Password[index]) == true) count = count + 1;
                    if (Char.IsSymbol(user.Password[index]) == true) count = count + 1;
                }
            }
            catch
            {
                //catch error empty field,display password not secure using if               
            }

            if (count >= 8)  flag = false;               
            else
               if (count < 8)
            {
                var get_resource_data = _localizer["SecurePass"];
                ViewData["SecurePass"] = get_resource_data;
                flag = true;
            }


            return flag;
        }
        public bool oldpass(UserData user)
        {
            bool flag = false;
          
            //create select query email use Entity Framework
            var password = _conString.UserData.Where(pass => pass.Password == user.Password)
                .FirstOrDefault();

            if (password != null)
            {
                flag = true;
                var get_resource_data = _localizer["Oldpass_true"];
                ViewData["Oldpass_true"] = get_resource_data;
            }
            return flag;
        }

        
    }
}
