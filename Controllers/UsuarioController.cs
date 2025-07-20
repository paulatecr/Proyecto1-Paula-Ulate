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

            // Deserializar preferencias si existen
            if (!string.IsNullOrEmpty(user.Preferencias))
            {
                try
                {
                    dynamic prefs = JsonConvert.DeserializeObject(user.Preferencias);
                    user.Tema = prefs.tema;
                    user.TamañoLetra = prefs.tamañoLetra;
                }
                catch { }
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult Editar(Usuario usuario)
        {
            var original = Session["UsuarioLogueado"] as Usuario;
            if (original == null)
                return RedirectToAction("Index", "Home");

            usuario.Id = original.Id;

            // Solo el administrador puede cambiar nombre/correo/rol
            if (original.Rol != "Administrador")
            {
                usuario.Nombre = original.Nombre;
                usuario.Correo = original.Correo;
                usuario.Rol = original.Rol;
            }

            // Validar y actualizar contraseña
            string nueva = Request.Form["NuevaContrasena"];
            string confirmar = Request.Form["ConfirmarContrasena"];

            if (!string.IsNullOrWhiteSpace(nueva))
            {
                if (nueva != confirmar)
                {
                    ViewBag.ErrorContrasena = "Las contraseñas no coinciden.";
                    usuario.Contraseña = original.Contraseña;
                    return View(usuario);
                }
                usuario.Contraseña = nueva;
            }
            else
            {
                usuario.Contraseña = original.Contraseña;
            }

            // Leer preferencias del formulario
            string tema = Request.Form["tema"];
            string tamañoLetra = Request.Form["tamañoLetra"];
            var preferencias = new { tema, tamañoLetra = tamañoLetra };
            usuario.Preferencias = JsonConvert.SerializeObject(preferencias);

            repositorio.Actualizar(usuario);

            // Actualizar sesión
            Session["UsuarioLogueado"] = usuario;

            ViewBag.Mensaje = "Perfil actualizado correctamente.";
            return View(usuario);
        }
    }
}