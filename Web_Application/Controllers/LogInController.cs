using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Web_Application.Models;
using static System.Text.Encodings.Web.HtmlEncoder;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;


namespace Web_Application.Controllers
{
    public class LogInController : Controller
    {

        private IHttpContextAccessor Accessor;


        public LogInController(IHttpContextAccessor _accessor)
        {
            this.Accessor = _accessor;
        }

        public IActionResult SignIn()
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            //Create a Cookie with a suitable Key and add the Cookie to Browser.
            Response.Cookies.Append("hide_layout", "false", option);


            ViewBag.hide_elements_layout = false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(UserData user)
        {

            SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            con.Open();
            string query_username = string.Format("select username from UserData " +
                "where username='{0}'", user.Username);
            string query_password = string.Format("select password from UserData " +
                "where username='{0}'", user.Username);
            string query_2fa = string.Format("select tfa from UserData " +
               "where username='{0}'", user.Username);

            SqlCommand cmd = new SqlCommand(query_username, con);
            SqlDataReader reader = cmd.ExecuteReader();





            SqlCommand cmd2 = new SqlCommand(query_2fa, con);
            SqlDataReader reader2 = cmd2.ExecuteReader();

            if (reader2.Read() == true)
            {
                                    
                if (reader2.GetString(0) == "1")
                {

                    if (reader.Read() == true)
                    {
                        reader.Close();

                        SqlCommand cmd1 = new SqlCommand(query_password, con);
                        SqlDataReader reader1 = cmd1.ExecuteReader();

                        if (reader1.Read() == true)
                        {
                            if (reader1.GetString(0) == user.Password)
                            {

                               

                                //Create a Cookie with a suitable Key and add the Cookie to Browser.
                                Response.Cookies.Append("UserName", user.Username, option);

                                return RedirectToAction("TFA_LogIn", "SignIn_TFA");
                            }

                        }
                    }
                }
                else
                   if (reader2.GetString(0) == "0")
                {
                    if (reader.Read() == true)
                    {
                        reader.Close();

                        SqlCommand cmd1 = new SqlCommand(query_password, con);
                        SqlDataReader reader1 = cmd1.ExecuteReader();

                        if (reader1.Read() == true)
                        {
                            if (reader1.GetString(0) == user.Password)
                            {

                               

                                //Create a Cookie with a suitable Key and add the Cookie to Browser.
                                Response.Cookies.Append("UserName", user.Username, option);

                                return RedirectToAction("Account", "Account");
                            }

                        }
                    }
                }
               
            }
            ViewData["Result"] = "Username or Password is incorect";


            con.Close();
            return View();
            }


        }
    }

