using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Final.Models;
using System.Web.Security;

namespace Final.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        

        //
        // GET: /Account/Login
        // Affiche la vue d'authentification
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {             
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        // Valide l'authentification et crée le cookie
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserName, string Password)
        {            
            if (Utilisateur.Authentifie(UserName, Password))
            {
                FormsAuthentication.SetAuthCookie(Utilisateur.getByUserName(UserName), false);
            }
            else
            {               
               ViewBag.error = "Nom d'utilisateur ou mot de passe invlaide";                                                               
               return View();
            }                
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        // Affiche la vue permettant à l'utilisateur de créer un compte.
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        // Crée un compte d'utilisateur
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Utilisateur u)
        {
            if (ModelState.IsValid)
            {
                Utilisateur.creer(u);
            }
            return RedirectToAction("Login");
        }

        

        //
        // GET: /Account/Manage
        // Affiche l'écran de modification du mot de passe
        public ActionResult Manage()
        {
            return View(Utilisateur.getByFullName(User.Identity.Name));
        }

        //
        // POST: /Account/Manage
        // Modifie le mot de passe de l'utilisateur
        [HttpPost]
        public ActionResult Manage(Utilisateur u)
        {
            Utilisateur.Modifier(u);
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/LogOff
        // Détruit le cookie d'authentification de l'utilisateur
        [HttpGet]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}