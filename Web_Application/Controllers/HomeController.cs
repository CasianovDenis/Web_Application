using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Web_Application.Models;

namespace Web_Application.Controllers
{
    public class HomeController : Controller
    {
        /* private readonly ILogger<HomeController> _logger;


         public HomeController(ILogger<HomeController> logger)
         {
             _logger = logger;
         }*/
        private IHttpContextAccessor Accessor;


        public HomeController(IHttpContextAccessor _accessor)
        {
            this.Accessor = _accessor;
        }

        

        public IActionResult Index()

        {
            string hide_layout = this.Accessor.HttpContext.Request.Cookies["hide_layout"];

            if (hide_layout=="true")
            {
                string username = this.Accessor.HttpContext.Request.Cookies["UserName"];

                ViewData["Username"] = username;


                SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

                con.Open();
                string query_email = string.Format("select email from UserData " +
                    "where username='{0}'", username);

                SqlCommand cmd = new SqlCommand(query_email, con);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read() == true)
                    ViewData["Email"] = reader.GetString(0);

                ViewBag.hide_elements_layout = Convert.ToBoolean(hide_layout);
               
            }
            return View();

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
