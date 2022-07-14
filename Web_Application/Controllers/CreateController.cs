using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Web_Application.Models;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Web_Application.Controllers
{
    public class CreateController : Controller
    {        
        private readonly ConString _conString;
        private readonly IHtmlLocalizer<CreateController> _localizer;

        public CreateController(ConString conection,IHtmlLocalizer<CreateController> localizer)
        {
            _conString = conection;
            _localizer = localizer;
        }

       
         public IActionResult SignUp()
         {
            var get_resource_data = _localizer["User_name"];
            ViewData["User_name"] = get_resource_data;

            get_resource_data = _localizer["Password"];
            ViewData["Password"] = get_resource_data;

            get_resource_data = _localizer["Mail"];
            ViewData["Mail"] = get_resource_data;

            get_resource_data = _localizer["SignUp"];
            ViewData["SignUp"] = get_resource_data;

            get_resource_data = _localizer["Create"];
            ViewData["Create"] = get_resource_data;

            return View();
         }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(UserData user)
        {
            if (existusername(user) == false)
            {
                if (existemail(user) == false)
                {
                    if (checkpass(user) == false)
                    {
                        user.TFA = "0";
                        _conString.Add(user);
                        _conString.SaveChanges();

                        var get_resource_data = _localizer["create_message"];
                        ViewData["create_message"] = get_resource_data;
                    }
                }
            }
            return View(user);
        }


        //check if the entered username  exists in the database
        public bool existusername(UserData user)
        {
            bool flag=false;

            var username = _conString.UserData.Where(name => name.Username == user.Username)
                .FirstOrDefault();
            
            if (username != null)
            { flag = true;
                var get_resource_data = _localizer["ExistUsername"];
                ViewData["ExistUsername"] = get_resource_data;
            }
            return flag;
        }

        //Сheck if the entered email exists in the database
        public bool existemail(UserData user)
        {
            bool flag = false;
            //create select query email use Entity Framework
            var username = _conString.UserData.Where(name => name.Email == user.Email)
                .FirstOrDefault();

            if (username != null)
            { flag = true;
                var get_resource_data = _localizer["ExistEmail"];
                ViewData["ExistEmail"] = get_resource_data;
            }
            return flag;
        }

        //Verification if insert password is security
        public bool checkpass(UserData user)
        {
            bool flag = false;
            int count = 0;

            try
            {
                for (int index = 0; index < user.Password.Length; index++)
                {
                    if (Char.IsLetter(user.Password[index]) == true) count = count + 1;
                    if (Char.IsNumber(user.Password[index]) == true) count = count + 1;
                    if (Char.IsUpper(user.Password[index]) == true) count = count + 1;
                    if (Char.IsSymbol(user.Password[index]) == true) count = count + 1;

                }
            }
            catch 
            {
                //catch error empty field,display password not secure using if        
            }
            if (count >= 8) flag = false; 
            else
               if (count < 8) {
                var get_resource_data = _localizer["SecurePass"];
                ViewData["SecurePass"] = get_resource_data; 
                flag = true; }


            return flag;
        }
    }
}
