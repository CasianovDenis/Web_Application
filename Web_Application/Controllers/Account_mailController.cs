using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Web_Application.Models;

namespace Web_Application.Controllers
{
    public class Account_mailController : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

        Guid random_secret_key = Guid.NewGuid();

        private IHttpContextAccessor Accessor;

        private string username;
        private readonly IHtmlLocalizer<Account_mailController> _localizer;

        public Account_mailController(IHttpContextAccessor _accessor, IHtmlLocalizer<Account_mailController> localizer)
        {
            this.Accessor = _accessor;
            _localizer = localizer;

        }

        public IActionResult Account_email()
        {
            //extract data from cookie storage
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];
            //set form display
            ViewBag.display_2fa = this.Accessor.HttpContext.Request.Cookies["status_2fa"];

            ViewData["Username"] = username;

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);
            
           

            con.Open();
            string query_email = string.Format("select email from UserData " +
                "where username='{0}'", username);

            SqlCommand cmd = new SqlCommand(query_email, con);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read() == true)
            {
                ViewData["Email"] = reader.GetString(0);
                ViewBag.email = reader.GetString(0);
            }
            con.Close();



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

            get_resource_data = _localizer["text_manage_email"];
            ViewData["text_manage_email"] = get_resource_data;

            get_resource_data = _localizer["new_email"];
            ViewData["new_email"] = get_resource_data;

            get_resource_data = _localizer["Change_email"];
            ViewData["Change_email"] = get_resource_data;

            get_resource_data = _localizer["warning_security"];
            ViewData["warning_security"] = get_resource_data;
            return View();
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Account_email(UserData user)
        {
            //if load page to call constructor page
            Account_email();

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

           
            //check if setting->email

           
       if (existemail(user) == false)
            {
                                
                SmtpClient Smtp = new SmtpClient("smtp.mail.ru", 587);
                Smtp.EnableSsl = true;
                Smtp.Credentials = new NetworkCredential("email@mail.ru", "password");//real email and password
                                                                                                          //was hide

                MailMessage Message = new MailMessage();
                Message.From = new MailAddress("email@mail.ru");//real email was hide

                Message.To.Add(new MailAddress(ViewBag.email));
                Message.Subject = "Change email";
                Message.Body = "Confirme if is you request to change email addres,insert this code " + random_secret_key +
                    " in application window." + "\n" +
                    "If is not you request,please change password in application.";

                Smtp.Send(Message);

                Response.Cookies.Append("secret_key_email", random_secret_key.ToString(), option);
                Response.Cookies.Append("new_email", user.Email, option);
                ViewData["WarningEmail"] = "Check you email";
                return RedirectToAction("Verification_email", "Verification");
            }

            return View("Account_email");
        }

        public bool existemail(UserData user)
        {
            bool flag = false;

            con.Open();
            string query_email = string.Format("select email from UserData " +
                "where email='{0}'", user.Email);

            SqlCommand cmd = new SqlCommand(query_email, con);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read() == true)
            {
                var get_resource_data = _localizer["WarningEmail"];
                ViewData["WarningEmail"] = get_resource_data;
                flag = true; }

            con.Close();

            return flag;
        }

       

       

    }
}
