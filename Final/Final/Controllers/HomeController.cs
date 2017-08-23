using Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Final.Controllers
{
    public class HomeController : Controller
    {
        // Affiche la vue Index de l'application      
        [Authorize]
        public ActionResult Index()
        {           
          return View();
        }

        // Si l'utilisateur est un conducteur, affiche la page des conducteurs
        [HttpGet]
        public ActionResult Conducteur()
        {
            return View("Conducteur", Voyage.getVoyageParConducteur(User.Identity.Name));
        }

        // Si l'utilisateur est un passager, affiche la page des passagers
        // Liste tous les voyages disponibles
        [HttpGet]
        public ActionResult Passagers()
        {
            return View("Passager", Voyage.getAllVoyages());
        }

        // Affiche la vue partielle de création d'un voyage.
        // Cette vue est affichée dans la vue principale des conducteurs
        [HttpGet]
        public ActionResult CreateVoyage()
        {
            return PartialView("CreateVoyage");
        }

        // Appelle la méthode de création d'un voyage quand le conducteur crée un nouveau voyage
        // Redirige le conducteur vers la page des conducteurs.
        [HttpPost]
        public ActionResult CreateVoyage(Voyage v)
        {
            if (v.Depart != null)
            {
                Voyage.creer(v);
                return RedirectToAction("Conducteur");
            }
                
            return View();
        }

        // Permet à un conducteur d'annuler un voyage qu'il avait proposé.
        public ActionResult Supprimer(int id)
        {
            if (Request.HttpMethod == "GET")
                return View(Voyage.getVoyageById(id));
            else
            {
                Voyage.Supprimer(id);
                return RedirectToAction("Conducteur");
            }
        }

        // Ajoute un voyage à la liste des voyages d'un passager et décrémente le nombre de places restantes.
        public ActionResult Choisir(int id)
        {
            Voyage.Choisir(id, Utilisateur.getByFullName(User.Identity.Name).Id);
            return RedirectToAction("Passagers");
        }

        // Affiche la vue partielle listant les voyages choisis par un passager
        // Cette vue partielle est affichée dans la vue principale des passagers
        public ActionResult ListerVoyages()
        {
            return PartialView("VoyagesChoisis", Voyage.getVoyageParPassager(User.Identity.Name));
        }

        // Permet à un passager d'annuler sa présence sur un voyage qu'il a choisi.
        public ActionResult Delete(int id)
        {
            if(Request.HttpMethod == "GET")
                return View(Voyage.getVoyageById(id));
            else
            {
                Passager.Delete(id);
                return RedirectToAction("Passagers");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}