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
    public class VerificationController : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");



        private IHttpContextAccessor Accessor;

        public VerificationController(IHttpContextAccessor _accessor)
        {
            this.Accessor = _accessor;

        }

        private string username;

        public ActionResult Verification_email()
        {
            //extract data from cookie storage
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];
            //set form display

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

        public ActionResult Verification_password()
        {
            //extract data from cookie storage
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];
            //set form display

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verification_email(UserData user)
        {
            Verification_email();

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);



            //check key
            string key_email = this.Accessor.HttpContext.Request.Cookies["secret_key_email"];

            if (user.Secretkey == key_email)
            {

                con.Open();
                user.Email = this.Accessor.HttpContext.Request.Cookies["new_email"];
                SqlCommand cmd = new SqlCommand("update UserData set email='" + user.Email + "' where username='" + username + "'", con);
                cmd.ExecuteNonQuery();
                con.Close();

                Response.Cookies.Append("open_form", "email", option);
                return RedirectToAction("Account_email", "Account_mail");
            }
            else
                ViewData["WarningVerification"] = "Key does not match";
            return View();


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verification_password(UserData user)
        {
            Verification_password();

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);



            string key_password = this.Accessor.HttpContext.Request.Cookies["secret_key_password"];
            string password = this.Accessor.HttpContext.Request.Cookies["password"];
            user.Password = password;

            
                if (user.Secretkey == key_password)
                {

                    con.Open();

                    SqlCommand cmd = new SqlCommand("update UserData set password='" + password + "' where username='" + username + "'", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    ViewData["WarningPassword"] = "Password was change";

                    Response.Cookies.Append("open_form", "password", option);
                   
                    return RedirectToAction("Account_password", "Account_pass");
                }
                else
                    
                return View();

            
        }
    }
}
