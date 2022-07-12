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
        SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

        string username;

        private IHttpContextAccessor Accessor;
        private readonly IHtmlLocalizer<Disable_2faController> _localizer;


        public Disable_2faController(IHttpContextAccessor _accessor, IHtmlLocalizer<Disable_2faController> localizer)
        {
            this.Accessor = _accessor;
            _localizer = localizer;
        }

        public IActionResult Disable_2fa()
        {
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];

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
