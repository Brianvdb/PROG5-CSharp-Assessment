using DomainModel;
using HotelWeb.Models;
using HotelWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HotelWeb.Controllers
{
    public class AccountController : Controller
    {
        private EntityUserAccountRepository accountRepo;

        public AccountController()
        {
            DatabaseContext db = new DatabaseContext();
            this.accountRepo = new EntityUserAccountRepository(db);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            string username = form["Username"];
            string password = form["Password"];
            bool success = false;
            foreach (UserAccount account in accountRepo.GetAll())
            {
                if (account.Username == username && account.Password == password)
                {
                    success = true;
                    break;
                }
            }
            if (!success)
            {
                ViewBag.Error = "Verkeerd gebruikersnaam en/of wachtwoord";
                return View();
            }
            bool remember = form["Remember"].StartsWith("true");
            FormsAuthentication.SetAuthCookie(username, remember);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }



    }
}
