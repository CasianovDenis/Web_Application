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
    public class SignIn_TFAController : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

        string username;

        private IHttpContextAccessor Accessor;
        private readonly IHtmlLocalizer<SignIn_TFAController> _localizer;


        public SignIn_TFAController(IHttpContextAccessor _accessor, IHtmlLocalizer<SignIn_TFAController> localizer)
        {
            this.Accessor = _accessor;
            _localizer = localizer;
        }

        public IActionResult TFA_LogIn()
        {
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];

            //Get data from localization and set
            var get_resource_data = _localizer["Text_confirme"];
            ViewData["Text_confirme"] = get_resource_data;

            get_resource_data = _localizer["Text_Introduce"];
            ViewData["Text_Introduce"] = get_resource_data;

            get_resource_data = _localizer["Text_pin"];
            ViewData["Text_pin"] = get_resource_data;

            get_resource_data = _localizer["SignIn"];
            ViewData["SignIn"] = get_resource_data;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TFA_LogIn(UserData user)
        {
            TFA_LogIn();
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


                if (status == true) return RedirectToAction("Account", "Account");

                else
                {
                    var get_resource_data = _localizer["Incorectpin"];
                    ViewData["Incorectpin"] = get_resource_data;
                }

                con.Close();
            }
            

            return View();
        }
    }
}
