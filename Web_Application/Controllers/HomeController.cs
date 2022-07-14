using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web_Application.Models;

namespace Web_Application.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHtmlLocalizer<HomeController> _localizer;
        private IHttpContextAccessor Accessor;
          private readonly ILogger<HomeController> _logger;
        private readonly ConString _conString;

        public HomeController(ConString conection, ILogger<HomeController> logger, IHttpContextAccessor _accessor, IHtmlLocalizer<HomeController> localizer)
          {
            _conString = conection;
            _logger = logger;
            this.Accessor = _accessor;
            _localizer = localizer;
          }

        public IActionResult Index()

        {
            string hide_layout = this.Accessor.HttpContext.Request.Cookies["hide_layout"];
            ViewBag.display_2fa = this.Accessor.HttpContext.Request.Cookies["status_2fa"];

            if (hide_layout=="true")
            {
                string username = this.Accessor.HttpContext.Request.Cookies["UserName"];

                ViewData["Username"] = username;               

                //create select query EF
                var user_db = _conString.UserData.Single(userdata=>userdata.Username == username);
                    ViewData["Email"] = user_db.Email;

                ViewBag.hide_elements_layout = Convert.ToBoolean(hide_layout);
               
            }
            return View();

        }

            [HttpPost]
        public IActionResult CultureManagement(string culture,string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(
                new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) }
                );
            return LocalRedirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
