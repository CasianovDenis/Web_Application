using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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



        public Account_mailController(IHttpContextAccessor _accessor)
        {
            this.Accessor = _accessor;
           
        }

        public IActionResult Account_email()
        {
            //extract data from cookie storage
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];
            //set form display
            ViewBag.open_form = this.Accessor.HttpContext.Request.Cookies["open_form"];
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
            return View();
        }

        public ActionResult redirect_password()
        {

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Append("open_form", "password", option);

            return RedirectToAction("Account_password", "Account_pass");

        }

        public ActionResult redirect_on_2fa()
        {

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Append("open_form", "tfa", option);

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

            if (existemail(user) == true) { Response.Cookies.Append("open_form", "email", option); }
            else
       if (existemail(user) == false)
            {
                                
                SmtpClient Smtp = new SmtpClient("smtp.mail.ru", 587);
                Smtp.EnableSsl = true;
                Smtp.Credentials = new NetworkCredential("username@gmail.com", "password");//real email and password
                                                                                                          //was hide

                MailMessage Message = new MailMessage();
                Message.From = new MailAddress("username@gmail.com");//real email was hide

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
            { ViewData["WarningEmail"] = "This Email already exist"; flag = true; }

            con.Close();

            return flag;
        }

       

       

    }
}
