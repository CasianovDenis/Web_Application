using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Web_Application.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text;

namespace Web_Application.Controllers
{
    public class Account_passController : Controller
    {

        SqlConnection con = new SqlConnection("Server=DESKTOP-EIMAL7F;Database=MyTable;Trusted_Connection=True;MultipleActiveResultSets=true");

        Guid random_secret_key = Guid.NewGuid();

     

        private IHttpContextAccessor Accessor;

        private string username;

       
        


        public Account_passController(IHttpContextAccessor _accessor)
        {
            this.Accessor = _accessor;
        }

        public IActionResult Account_password()
        {
            //var storage = new LocalStorage();
            //extract data from cookie storage
            username = this.Accessor.HttpContext.Request.Cookies["UserName"];
          

            ViewData["Username"] = username;

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Append("open_form", "password", option);
            //set form display
            ViewBag.open_form = this.Accessor.HttpContext.Request.Cookies["open_form"];

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
        public ActionResult Account_password(UserData user)
        {
      
            //if load page to call constructor page
            Account_password();

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);
            
            //check key

            string key_password = this.Accessor.HttpContext.Request.Cookies["secret_key_password"];
           string password = this.Accessor.HttpContext.Request.Cookies["password"];
            user.Password = password;

            if (ViewBag.open_form == "verification")
            {
                if (user.Secretkey == key_password)
                {
                    
                    con.Open();

                    SqlCommand cmd = new SqlCommand("update UserData set password='" + password + "' where username='" + username + "'", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    ViewData["WarningPassword"] = "Password was change";

                    Response.Cookies.Append("open_form", "password", option);
                }
            }

            //check if setting->password
            if (oldpass(user) == true) { Response.Cookies.Append("open_form", "password", option); }
           else
            {
                if (oldpass(user) == false)
                {
                    if (checkpass(user) == false)
                    {
                        Response.Cookies.Append("open_form", "verification", option);
                        ViewBag.open_form = this.Accessor.HttpContext.Request.Cookies["open_form"];

                        SmtpClient Smtp = new SmtpClient("smtp.mail.ru", 587);
                        Smtp.EnableSsl = true;
                        Smtp.Credentials = new NetworkCredential("username@gmail.com", "password");//real email and password
                                                                                                                  //was hide

                        MailMessage Message = new MailMessage();
                        Message.From = new MailAddress("username@gmail.com");//real email was hide

                        Message.To.Add(new MailAddress(ViewBag.email));
                        Message.Subject = "Change password";
                        Message.Body = "Confirme if is you request to change password ,insert this code " + random_secret_key +
                            " in application window." + "\n" +
                            "If is not you request,please change password in application.";

                        Smtp.Send(Message);

                        Response.Cookies.Append("secret_key_password", random_secret_key.ToString(), option);


                      

                        Response.Cookies.Append("password",user.Password, option);
                        

                        ViewData["WarningPassword"] = "Check you email";
                    }
                    else
                        ViewData["WarningPassword"] = "Password is not secure";

                }
            }

            return View("Account_password");
        }


        public bool checkpass(UserData user)
        {
            bool flag = false;
            int count = 0;
            try
            {
                char[] passchar = user.Password.ToCharArray();

                for (int index = 0; index < user.Password.Length; index++)
                {
                    if (Char.IsLetter(passchar[index]) == true) count = count + 1;
                    if (Char.IsNumber(passchar[index]) == true) count = count + 1;
                    if (Char.IsUpper(passchar[index]) == true) count = count + 1;
                    if (Char.IsSymbol(passchar[index]) == true) count = count + 1;

                }
            }
            catch
            {
                ViewData["WarningPassword"] = "Password is empty";
            }
            if (count >= 8) flag = false;
            else
               if (count < 8)
            {
                ViewData["WarningPass"] = "Password is not secure nedeed 6+ word,uppercase and number";
                flag = true;
            }


            return flag;
        }
        public bool oldpass(UserData user)
        {
            bool flag = false;

            con.Open();
            string query_password = string.Format("select password from UserData " +
                "where username='{0}'", username);

            SqlCommand cmd = new SqlCommand(query_password, con);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read() == true)
                if (reader.GetString(0) == user.Password) { ViewData["WarningPassword"] = "Password matches the old one"; flag = true; }
            con.Close();

            return flag;
        }
    }
}
