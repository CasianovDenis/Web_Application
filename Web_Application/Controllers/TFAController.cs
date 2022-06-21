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
    public class TFAController : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

        Guid random_secret_key = Guid.NewGuid();

        private IHttpContextAccessor Accessor;

        private string username;




        public TFAController(IHttpContextAccessor _accessor)
        {
            this.Accessor = _accessor;
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

    }
    }
