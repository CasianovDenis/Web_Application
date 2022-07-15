using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Application.Models;

namespace Web_Application.Controllers
{
    public class HelpController : Controller
    {
        private readonly IHtmlLocalizer<HelpController> _localizer;
        private IHttpContextAccessor Accessor;
        private readonly ILogger<HelpController> _logger;
        private readonly ConString _conString;

        public HelpController(ConString conection, ILogger<HelpController> logger, IHttpContextAccessor _accessor, IHtmlLocalizer<HelpController> localizer)
        {
            _conString = conection;
            _logger = logger;
            this.Accessor = _accessor;
            _localizer = localizer;
        }

        public IActionResult Help()
        {
            string hide_layout = this.Accessor.HttpContext.Request.Cookies["hide_layout"];
            ViewBag.display_2fa = this.Accessor.HttpContext.Request.Cookies["status_2fa"];

            if (hide_layout == "true")
            {
                string username = this.Accessor.HttpContext.Request.Cookies["UserName"];

                ViewData["Username"] = username;

                //create select query EF
                var user_db = _conString.UserData.Single(userdata => userdata.Username == username);
                ViewData["Email"] = user_db.Email;

                ViewBag.hide_elements_layout = Convert.ToBoolean(hide_layout);



            }

            var get_resource_data = _localizer["pin_error"];
            ViewData["pin_error"] = get_resource_data;

            get_resource_data = _localizer["Insert_pin"];
            ViewData["Insert_pin"] = get_resource_data;

            get_resource_data = _localizer["qr_error"];
            ViewData["qr_error"] = get_resource_data;

            get_resource_data = _localizer["insert_qr"];
            ViewData["insert_qr"] = get_resource_data;

            get_resource_data = _localizer["not_answer"];
            ViewData["not_answer"] = get_resource_data;

            get_resource_data = _localizer["support_answer"];
            ViewData["support_answer"] = get_resource_data;

            get_resource_data = _localizer["help_center"];
            ViewData["help_center"] = get_resource_data;

            return View();
        }
    }
}
