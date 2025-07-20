using Proyecto1_Paula_Ulate.LogicaDatos;
using Proyecto1_Paula_Ulate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string usuario, string contrasena)
        {
            var repo = new UsuarioRepository(ConfigurationManager.ConnectionStrings["ConexionBaseDatos"].ConnectionString);
            var usuarios = repo.ObtenerTodos();
            var user = usuarios.FirstOrDefault(u => u.UsuarioID == usuario && u.Contraseña == contrasena);

            if (user != null)
            {
                Session["UsuarioLogueado"] = user;
                return RedirectToAction("Perfil", "Usuario");
            }
            else
            {
                ViewBag.Mensaje = "Credenciales incorrectas.";
                return View("Index");
            }
        }

        public ActionResult CerrarSesion()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }



    }
}