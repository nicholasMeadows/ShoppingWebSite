using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using Shopping.Models;
using Shopping.Context;
using Shopping.Helpers;
namespace Shopping.Controllers
{
    public class WebPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(UserPassModel userPass)
        {
            //string userString = HttpContext.Session.GetString("user");
            if(userPass.username == null) { 
                ModelState.Remove("username");
                ModelState.Remove("password");
                //auto sign in
                return View("Login");
            }
            else {

                if (ServerSideValidation.ValidateInfo(userPass))
                {
                    if (UserDatabaseContext.Login(userPass))
                    {
                        //Use Db context to validate user name and pass
                        TempData["User"] = userPass.username;

                        User user = new User();
                        user.username = userPass.username;

                        user.accessToken = UserDatabaseContext.GenerateAccessToken(userPass.username);

                        HttpContext.Session.SetString("user", JsonConvert.SerializeObject(user));

                        return RedirectToAction("Index");
                    }
                    else {
                        TempData["err"] = "Invalid username or password";
                        return View();
                    }
                }
                return View();
            }
            
            
        }

        public IActionResult Register(RegisterUserModel registerUser)
        {
            if (registerUser.username == null)
            {
                ModelState.Remove("username");
                ModelState.Remove("password");
                ModelState.Remove("confirmPassword");

                return View("Register");
            }
            else
            {
                //User Db context to register user
                if (UserDatabaseContext.CheckUsername(registerUser.username))
                {                    
                    UserDatabaseContext.Register(registerUser);
                    User user = new User();
                    user.username = registerUser.username;
                    
                    user.accessToken = UserDatabaseContext.GenerateAccessToken(registerUser.username);

                    HttpContext.Session.SetString("user", JsonConvert.SerializeObject(user));
                }
                else {
                    TempData["err"] = "Username already exists";
                    return View("Register");                    
                }
            }

            UserPassModel userPass = new UserPassModel();
            userPass.username = registerUser.username;
            userPass.password = registerUser.password;

            return RedirectToAction("Login", userPass);
        }

        public IActionResult SignOut() {
            TempData.Clear();
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
