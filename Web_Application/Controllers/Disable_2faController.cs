using Google.Authenticator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

        string username;

        private IHttpContextAccessor Accessor;


        public Disable_2faController(IHttpContextAccessor _accessor)
        {
            this.Accessor = _accessor;
        }

        public IActionResult Disable_2fa()
        {
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disable_2fa(UserData user)
        {
            Disable_2fa();
            string query_key = string.Format("select secretkey from UserData " +
                "where username='{0}'", username);

            con.Open();
            SqlCommand cmd = new SqlCommand(query_key, con);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read() == true)
            {
                string google_key = reader.GetString(0);


                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

                string pin = user.TFA.Trim();

                bool status = tfa.ValidateTwoFactorPIN(google_key, pin, TimeSpan.FromSeconds(30));


                if (status == true)
                {
                   
                   
                    SqlCommand cmd1 = new SqlCommand("update UserData set tfa='" + 0 + "' where username='" + username + "'", con);
                    cmd1.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand("update UserData set secretkey='" + 0 + "' where username='" + username + "'", con);
                    cmd2.ExecuteNonQuery();
                    

                    return RedirectToAction("TFA", "TFA"); }

                else
                    ViewData["TFAWarning"] = "Pin isn't corect";

                con.Close();
            }


            return View();
        }

        public ActionResult redirect_email()
        {

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Append("open_form", "email", option);

            return RedirectToAction("Account_email", "Account_mail");

        }

        public ActionResult redirect_password()
        {

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Append("open_form", "password", option);

            return RedirectToAction("Account_password", "Account_pass");

        }

    }
}
