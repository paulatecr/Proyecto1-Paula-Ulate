using System;
using System.Web.Mvc;
using Proyecto1_Paula_Ulate.Models;
using Proyecto1_Paula_Ulate.LogicaDatos;
using System.Configuration;
using Newtonsoft.Json;

namespace Proyecto1_Paula_Ulate.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioRepositorio repositorio;

        public UsuarioController()
        {
            var connectionString = ConfigurationManager
                .ConnectionStrings["ConexionBaseDatos"]
                .ConnectionString;

            repositorio = new UsuarioRepositorio(connectionString);
        }

        public ActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Crear(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                repositorio.Insertar(usuario);
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        public ActionResult Perfil()
        {
            var user = Session["UsuarioLogueado"] as Usuario;
            if (user == null)
                return RedirectToAction("Index", "Home");

            return View(user);
        }

        public ActionResult Editar()
        {
            var user = Session["UsuarioLogueado"] as Usuario;
            if (user == null)
                return RedirectToAction("Index", "Home");

            return View(user);
        }

        [HttpPost]
        public ActionResult Editar(Usuario usuario)
        {
            var original = Session["UsuarioLogueado"] as Usuario;
            if (original == null)
                return RedirectToAction("Index", "Home");

            usuario.Id = original.Id;

            // Solo los administradores pueden cambiar nombre/correo
            if (original.Rol != "Administrador")
            {
                usuario.Nombre = original.Nombre;
                usuario.Correo = original.Correo;
                usuario.Rol = original.Rol;
            }

            // Contraseña
            string nueva = Request.Form["NuevaContraseña"];
            string confirmar = Request.Form["ConfirmarContraseña"];

            if (!string.IsNullOrWhiteSpace(nueva))
            {
                if (nueva != confirmar)
                {
                    ViewBag.ErrorContraseña = "Las contraseñas no coinciden.";
                    return View(usuario);
                }

                usuario.Contraseña = nueva;
            }
            else
            {
                usuario.Contraseña = original.Contraseña;
            }

            // Preferencias
            var tema = Request.Form["tema"];
            var tamañoLetra = Request.Form["tamañoLetra"];
            var preferencias = new { tema, tamañoLetra };
            usuario.Preferencias = JsonConvert.SerializeObject(preferencias);

            repositorio.Actualizar(usuario);
            Session["UsuarioLogueado"] = usuario;
            ViewBag.Mensaje = "Perfil actualizado correctamente.";

            return View(usuario);
        }
    }
}