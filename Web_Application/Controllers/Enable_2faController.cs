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
    public class Enable_2faController : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

        Guid random_secret_key = Guid.NewGuid();

        private IHttpContextAccessor Accessor;

        private string username;

        private string google_key;


        public Enable_2faController(IHttpContextAccessor _accessor)
        {
            this.Accessor = _accessor;
        }
        public IActionResult Enable_2fa()
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
                ViewData["Email"] = reader.GetString(0);

            //Create a Cookie with a suitable Key and add the Cookie to Browser.
            Response.Cookies.Append("hide_layout", "true", option);
            //if log in hide unnecessary element
            ViewBag.hide_elements_layout = true;



            string query_2fa = string.Format("select tfa from UserData " +
                "where username='{0}'", username);

            SqlCommand cmd1 = new SqlCommand(query_2fa, con);
            SqlDataReader reader1 = cmd1.ExecuteReader();

            if (reader1.Read() == true)
            {
                if (reader1.GetString(0) == "0") { ViewBag.display = 0; 
                    string key = Guid.NewGuid().ToString().Replace("-", "").Replace("8", "").Replace("9", "").Substring(0, 16);
                    google_key = key;
                    Response.Cookies.Append("google_key", google_key, option);
                }
                else
                         if (reader1.GetString(0) == "1") ViewBag.display = 1;
            }
            con.Close();

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode("Test Two Factor", username.Trim(), google_key, false, 3);
            ViewData["tfa_key"] = setupInfo.ManualEntryKey;
            string qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl; //  assigning the Qr code information + URL to string
            ViewBag.qr_code = qrCodeImageUrl;// showing the qr code on the page "linking the string to image element"


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Enable_2fa(UserData user)
        {

            Enable_2fa();
            google_key= this.Accessor.HttpContext.Request.Cookies["google_key"];
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

            string pin = user.TFA;

            bool status = tfa.ValidateTwoFactorPIN(google_key, pin, TimeSpan.FromSeconds(30));

            if (status == true)
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("update UserData set tfa='1' where username='" + username + "'", con);
                cmd.ExecuteNonQuery();

                SqlCommand cmd1 = new SqlCommand("update UserData set secretkey='" + google_key + "' where username='" + username + "'", con);
                cmd1.ExecuteNonQuery();


                con.Close();

                return RedirectToAction("TFA", "TFA");
            }
            else
                ViewData["TFAWarning"] = "Pin isn't corect";
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
