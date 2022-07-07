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
    public class TFAController : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

        Guid random_secret_key = Guid.NewGuid();

        private IHttpContextAccessor Accessor;

        private string username;
        private readonly IHtmlLocalizer<TFAController> _localizer;

        public TFAController(IHttpContextAccessor _accessor, IHtmlLocalizer<TFAController> localizer)
        {
            this.Accessor = _accessor;
            _localizer = localizer;

        }

        public IActionResult TFA()
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
                if (reader1.GetString(0) == "0") { ViewBag.display = 0; ViewData["2FA"] = "Disable"; }
                else
                         if (reader1.GetString(0) == "1") { ViewBag.display = 1; ViewData["2FA"] = "Enable"; }
            }
            con.Close();


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

            get_resource_data = _localizer["text_manage_tfa"];
            ViewData["text_manage_tfa"] = get_resource_data;

            get_resource_data = _localizer["Enable_tfa"];
            ViewData["Enable_tfa"] = get_resource_data;

            get_resource_data = _localizer["Disable_tfa"];
            ViewData["Disable_tfa"] = get_resource_data;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TFA(UserData user)
        {
            TFA();
            if (ViewBag.display == 0) return RedirectToAction("Enable_2fa", "Enable_2fa");
            else
            if (ViewBag.display == 1) return RedirectToAction("Disable_2fa", "Disable_2fa");
            else
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
