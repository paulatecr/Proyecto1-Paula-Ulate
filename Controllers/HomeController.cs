using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto1_Paula_Ulate.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string usuario, string contrasena)
        {
            // Ejemplo básico. Cambia esto luego por validación real desde la base de datos
            if (usuario == "admin" && contrasena == "1234")
            {
                Session["Usuario"] = usuario;
                return RedirectToAction("Index", "Usuario");
            }

            ViewBag.Mensaje = "Credenciales inválidas. Intente de nuevo.";
            return View("Index");
        }

        public ActionResult CerrarSesion()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}