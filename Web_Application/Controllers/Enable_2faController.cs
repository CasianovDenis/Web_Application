﻿using Google.Authenticator;
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
    public class Enable_2faController : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

        Guid random_secret_key = Guid.NewGuid();

        private IHttpContextAccessor Accessor;

        private string username;

        private string google_key;
        private readonly IHtmlLocalizer<Enable_2faController> _localizer;

        public Enable_2faController(IHttpContextAccessor _accessor, IHtmlLocalizer<Enable_2faController> localizer)
        {
            this.Accessor = _accessor;
            _localizer = localizer;
        }
        public IActionResult Enable_2fa()
        {
            //extract data from cookie storage
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];

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

            //Get data from localization and set
            var get_resource_data = _localizer["TextManage"];
            ViewData["TextManage"] = get_resource_data;

            get_resource_data = _localizer["TextChange"];
            ViewData["TextChange"] = get_resource_data;

            get_resource_data = _localizer["Text_download"];
            ViewData["Text_download"] = get_resource_data;

            get_resource_data = _localizer["Text_scan"];
            ViewData["Text_scan"] = get_resource_data;

            get_resource_data = _localizer["Text_introduce"];
            ViewData["Text_introduce"] = get_resource_data;

            get_resource_data = _localizer["Confirme"];
            ViewData["Confirme"] = get_resource_data;

            get_resource_data = _localizer["Profile"];
            ViewData["Profile"] = get_resource_data;

            get_resource_data = _localizer["Mail"];
            ViewData["Mail"] = get_resource_data;

            get_resource_data = _localizer["Password"];
            ViewData["Password"] = get_resource_data;

            get_resource_data = _localizer["tfa"];
            ViewData["tfa"] = get_resource_data;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Enable_2fa(UserData user)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

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

                ViewBag.display_2fa = "1"; 
                Response.Cookies.Append("status_2fa", "1", option);

                return RedirectToAction("TFA", "TFA");
            }
            else
                ViewData["TFAWarning"] = "Pin isn't corect";
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

        [HttpPost]
        public ActionResult redirect_on_2fa()
        {
            return RedirectToAction("TFA", "TFA");

        }
    }
}
