using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Net;

namespace Web_Application.Controllers
{
    public class AccountController : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

        Guid random_secret_key = Guid.NewGuid();

        private IHttpContextAccessor Accessor;

        private string username;

       

        public AccountController(IHttpContextAccessor _accessor)
        {
            this.Accessor = _accessor;
        }

        public IActionResult Account()
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
                
            
            con.Close();
           


            //Create a Cookie with a suitable Key and add the Cookie to Browser.
            Response.Cookies.Append("hide_layout", "true", option);
            //if log in hide unnecessary element
            ViewBag.hide_elements_layout = true;
            return View();
        }


          [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Account(UserData user)
        {
            Account();
            return View("Account"); 
        }

       

       
    } 
}
