using Proyecto1_Paula_Ulate.Models;
using Proyecto1_Paula_Ulate.LogicaDatos;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Proyecto1_Paula_Ulate.Controllers
{
    public class SolicitudController : Controller
    {
        private readonly SolicitudRepository repositorio;
        private readonly RepuestoRepository repuestoRepo;

        public SolicitudController()
        {
            repositorio = new SolicitudRepository();
            repuestoRepo = new RepuestoRepository(System.Configuration.ConfigurationManager.ConnectionStrings["ConexionBaseDatos"].ConnectionString);
        }

        // GET: Solicitud
        public ActionResult Index()
        {
            var solicitudes = repositorio.ObtenerTodos();
            return View(solicitudes);
        }

        // GET: Solicitud/Crear?repuestoId=3
        public ActionResult Crear(int? repuestoId)
        {
            var usuario = Session["UsuarioLogueado"] as Usuario;
            if (usuario == null)
                return RedirectToAction("Index", "Home");

            if (repuestoId.HasValue)
            {
                var repuesto = repuestoRepo.ObtenerPorId(repuestoId.Value);
                ViewBag.Repuesto = repuesto;
            }
            else
            {
                ViewBag.Repuesto = null;
            }

            var solicitud = new Solicitud
            {
                RepuestoId = repuestoId ?? 0,
                FechaSolicitud = DateTime.Now,
                Solicitante = usuario.UsuarioID
            };

            return View(solicitud);
        }

        // POST: Solicitud/Crear
        [HttpPost]
        public ActionResult Crear(Solicitud solicitud)
        {
            var usuario = Session["UsuarioLogueado"] as Usuario;
            if (usuario == null)
                return RedirectToAction("Index", "Home");

            var repuesto = repuestoRepo.ObtenerPorId(solicitud.RepuestoId);
            if (repuesto == null)
            {
                TempData["Error"] = "Repuesto no encontrado.";
                return RedirectToAction("Index", "Repuesto");
            }

            solicitud.Solicitante = usuario.UsuarioID;
            solicitud.FechaSolicitud = DateTime.Now;
            solicitud.Estado = "Nueva";

            if (solicitud.CantidadSolicitada > repuesto.CantidadDisponible)
            {
                TempData["Advertencia"] = "No hay suficiente cantidad. Se procesará parcialmente.";
            }

            repositorio.Insertar(solicitud);

            TempData["Mensaje"] = "Solicitud creada exitosamente.";
            return RedirectToAction("Index");
        }

        /// GET: Solicitud/Editar
        public ActionResult Editar(int id)
        {
            var solicitud = repositorio.BuscarPorId(id);
            if (solicitud == null)
            {
                TempData["Error"] = "Solicitud no encontrada.";
                return RedirectToAction("Index");
            }

            // Obtener el repuesto actual de la solicitud
            var repuestoActual = repuestoRepo.ObtenerPorId(solicitud.RepuestoId);
            solicitud.Repuesto = repuestoActual;

            // Para el dropdown con todos los repuestos
            ViewBag.Repuestos = repuestoRepo.ObtenerTodos();

            return View(solicitud);
        }

        // POST: Solicitud/Editar
        [HttpPost]
        public ActionResult Editar(Solicitud solicitud)
        {
            var usuario = Session["UsuarioLogueado"] as Usuario;
            if (usuario == null)
                return RedirectToAction("Index", "Home");

            solicitud.Solicitante = usuario.UsuarioID; // Asignar solicitante aquí

            try
            {
                repositorio.Actualizar(solicitud);
                TempData["Mensaje"] = "Solicitud actualizada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar: " + ex.Message;
                ViewBag.Repuestos = repuestoRepo.ObtenerTodos(); // Si quieres que vuelva a cargar la vista con el drop
                return View(solicitud);
            }
        }

        // GET: Solicitud/CrearDesdeBoton
        public ActionResult CrearDesdeBoton()
        {
            var usuario = Session["UsuarioLogueado"] as Usuario;
            if (usuario == null)
                return RedirectToAction("Index", "Home");

            ViewBag.Repuestos = repuestoRepo.ObtenerTodos();

            var solicitud = new Solicitud
            {
                FechaSolicitud = DateTime.Now,
                Solicitante = usuario.UsuarioID
            };

            return View(solicitud); // Vista: CrearDesdeBoton.cshtml
        }

        // POST: Solicitud/CrearDesdeBoton
        [HttpPost]
        public ActionResult CrearDesdeBoton(Solicitud solicitud)
        {
            var usuario = Session["UsuarioLogueado"] as Usuario;
            if (usuario == null)
                return RedirectToAction("Index", "Home");

            var repuesto = repuestoRepo.ObtenerPorId(solicitud.RepuestoId);
            if (repuesto == null)
            {
                TempData["Error"] = "Repuesto no encontrado.";
                return RedirectToAction("Index");
            }

            solicitud.Solicitante = usuario.UsuarioID;
            solicitud.FechaSolicitud = DateTime.Now;
            solicitud.Estado = "Nueva";

            if (solicitud.CantidadSolicitada > repuesto.CantidadDisponible)
                TempData["Advertencia"] = "No hay suficiente cantidad. Se procesará parcialmente.";

            repositorio.Insertar(solicitud);
            TempData["Mensaje"] = "Solicitud creada correctamente.";
            return RedirectToAction("Index");
        }

        // GET: Solicitud/Eliminar
        public ActionResult Eliminar(int id)
        {
            var solicitud = repositorio.BuscarPorId(id);
            if (solicitud == null)
                return HttpNotFound();

            repositorio.Eliminar(id);
            TempData["Mensaje"] = "Solicitud eliminada correctamente.";
            return RedirectToAction("Index");
        }
    }
}
