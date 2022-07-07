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
        SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

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
                       
                        _conString.Add(user);
                        _conString.SaveChanges();


                        con.Open();
                            SqlCommand cmd = new SqlCommand("update UserData set tfa='" + 0 + "' where username='" + user.Username + "'", con);
                            cmd.ExecuteNonQuery();
                            con.Close();
                      
                        ViewBag.message = "Account created successfully";
                    }
                }
            }
            return View(user);
        }


        //check if the entered username  exists in the database
        public bool existusername(UserData user)
        {
            bool flag=false;
           
            SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");
            
            con.Open();
            string query_username = string.Format("select username from UserData " +
                "where username='{0}'", user.Username);


            SqlCommand cmd = new SqlCommand(query_username, con);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read() == true)
            {
                var get_resource_data = _localizer["ExistUsername"];
                ViewData["ExistUsername"] = get_resource_data;
                flag = true; }

            con.Close();

            return flag;
        }

        //Сheck if the entered email exists in the database
        public bool existemail(UserData user)
        {
            bool flag = false;

            SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

            con.Open();
            string query_email = string.Format("select email from UserData " +
                "where email='{0}'", user.Email);

            SqlCommand cmd = new SqlCommand(query_email, con);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read() == true)
            {
                var get_resource_data = _localizer["ExistEmail"];
                ViewData["ExistEmail"] = get_resource_data; 
                 flag = true; }

            con.Close();

            return flag;
        }

        //Verification if insert password is security
        public bool checkpass(UserData user)
        {
            bool flag = false;
            int count = 0;

            char[] passchar = user.Password.ToCharArray(); 

            for (int index=0;index<user.Password.Length;index++)
            {
                if (Char.IsLetter(passchar[index]) == true) count = count + 1;
                if (Char.IsNumber(passchar[index]) == true) count = count + 1;
                if (Char.IsUpper(passchar[index]) == true) count = count + 1;
                if (Char.IsSymbol(passchar[index]) == true) count = count + 1;

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
