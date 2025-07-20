using Proyecto1_Paula_Ulate.Models;
using Proyecto1_Paula_Ulate.LogicaDatos;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            repositorio.Eliminar(id);
            return RedirectToAction("Index");
        }
    }
}