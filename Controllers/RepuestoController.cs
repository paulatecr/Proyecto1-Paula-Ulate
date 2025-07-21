using Proyecto1_Paula_Ulate.LogicaDatos;
using Proyecto1_Paula_Ulate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Proyecto1_Paula_Ulate.Controllers
{
    public class RepuestoController : Controller
    {
        private readonly RepuestoRepository repositorio;

        public RepuestoController()
        {
            var connectionString = ConfigurationManager
                .ConnectionStrings["ConexionBaseDatos"]
                .ConnectionString;

            repositorio = new RepuestoRepository(connectionString);
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
        public ActionResult Crear(Repuesto repuesto)
        {
            if (!ModelState.IsValid)
                return View(repuesto);

            repositorio.Insertar(repuesto);
            return RedirectToAction("Index");
        }

        public ActionResult Editar(int id)
        {
            var repuesto = repositorio.ObtenerPorId(id);
            if (repuesto == null)
                return HttpNotFound();

            return View(repuesto);
        }

        [HttpPost]
        public ActionResult Editar(Repuesto repuesto)
        {
            if (!ModelState.IsValid)
                return View(repuesto);

            repositorio.Actualizar(repuesto);
            return RedirectToAction("Index");
        }

        public ActionResult RegistrarEntrada()
        {
            var usuario = Session["UsuarioLogueado"] as Usuario;
            if (usuario == null || (usuario.Rol != "Administrador" && usuario.Rol != "Encargado"))
            {
                return RedirectToAction("Index", "Home");
            }

            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        public ActionResult RegistrarCantidad(int id, int cantidad)
        {
            if (cantidad <= 0)
            {
                TempData["Error"] = "La cantidad debe ser mayor que cero.";
                return RedirectToAction("RegistrarEntrada");
            }

            var repuesto = repositorio.ObtenerPorId(id);
            if (repuesto == null)
            {
                TempData["Error"] = "El repuesto no existe.";
                return RedirectToAction("RegistrarEntrada");
            }

            repuesto.CantidadDisponible += cantidad;
            repositorio.ActualizarCantidad(repuesto); // <- esto llama al repositorio, no usa _connectionString

            TempData["Mensaje"] = $"Se han registrado {cantidad} unidades adicionales al repuesto {repuesto.Nombre}.";
            return RedirectToAction("RegistrarEntrada");
        }

        public ActionResult Eliminar(int id)
        {
            var usuario = Session["UsuarioLogueado"] as Usuario;
            if (usuario == null || (usuario.Rol != "Administrador" && usuario.Rol != "Encargado"))
            {
                return RedirectToAction("Index", "Home");
            }

            var repuesto = repositorio.ObtenerPorId(id);
            if (repuesto == null)
            {
                TempData["Error"] = "El repuesto no fue encontrado.";
                return RedirectToAction("Index");
            }

            repositorio.Eliminar(id);
            TempData["Mensaje"] = $"Repuesto \"{repuesto.Nombre}\" eliminado exitosamente.";
            return RedirectToAction("Index");
        }
    }
}