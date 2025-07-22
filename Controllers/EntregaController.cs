using Proyecto1_Paula_Ulate.LogicaDatos;
using Proyecto1_Paula_Ulate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Proyecto1_Paula_Ulate.Controllers
{
    public class EntregaController : Controller
    {
        private readonly EntregaRepository entregaRepo;
        private readonly SolicitudRepository solicitudRepo;
        private readonly RepuestoRepository repuestoRepo;
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConexionBaseDatos"].ConnectionString;

        public EntregaController()
        {
            entregaRepo = new EntregaRepository();
            solicitudRepo = new SolicitudRepository();
            repuestoRepo = new RepuestoRepository(System.Configuration.ConfigurationManager.ConnectionStrings["ConexionBaseDatos"].ConnectionString);
        }

        // GET: Entrega/Crear?sId=1
        public ActionResult Crear(int sId)
        {
            var usuario = Session["UsuarioLogueado"] as Usuario;
            if (usuario == null || (usuario.Rol != "Administrador" && usuario.Rol != "Encargado"))
                return RedirectToAction("Index", "Home");

            var solicitud = solicitudRepo.BuscarPorId(sId);
            if (solicitud == null)
            {
                TempData["Error"] = "Solicitud no encontrada.";
                return RedirectToAction("Index", "Solicitud");
            }

            var repuesto = repuestoRepo.ObtenerPorId(solicitud.RepuestoId);

            var entregaRepo = new EntregaRepository();
            var entregas = entregaRepo.ObtenerPorSolicitudId(sId) ?? new List<Entrega>();

            int totalEntregado = entregas.Sum(e => e.CantidadEntregada);
            int cantidadPendiente = solicitud.CantidadSolicitada - totalEntregado;
            //int cantidadPendiente = 2;

            ViewBag.Repuesto = repuesto;
            ViewBag.Solicitud = solicitud;
            ViewBag.CantidadPendiente = cantidadPendiente;

            return View(new Entrega
            {
                SolicitudId = sId,
                FechaEntrega = DateTime.Now,
                EntregadoPor = usuario.UsuarioID
            });
        }

        // POST: Entrega/Crear
        [HttpPost]
        public ActionResult Crear(Entrega entrega)
        {
            var usuario = Session["UsuarioLogueado"] as Usuario;
            if (usuario == null || (usuario.Rol != "Administrador" && usuario.Rol != "Encargado"))
                return RedirectToAction("Index", "Home");

            var solicitud = solicitudRepo.BuscarPorId(entrega.SolicitudId);
            var repuesto = repuestoRepo.ObtenerPorId(solicitud.RepuestoId);

            // Validaciones
            if (entrega.CantidadEntregada <= 0)
            {
                TempData["Error"] = "La cantidad entregada debe ser mayor a cero.";
                return RedirectToAction("Crear", new { sId = entrega.SolicitudId });
            }

            if (entrega.CantidadEntregada > repuesto.CantidadDisponible)
            {
                TempData["Error"] = "No hay suficiente repuesto en bodega para esta entrega.";
                return RedirectToAction("Crear", new { sId = entrega.SolicitudId });
            }

            // Actualizar repuesto
            repuesto.CantidadDisponible -= entrega.CantidadEntregada;
            repuestoRepo.Actualizar(repuesto);

            // Insertar entrega
            entrega.FechaEntrega = DateTime.Now;
            entrega.EntregadoPor = usuario.UsuarioID;
            entregaRepo.Insertar(entrega);

            var usuarioRepo = new UsuarioRepository(connectionString);
            var usuariosMecanicos = usuarioRepo.ObtenerTodos()
                .Where(u => u.Rol == "Mecanico").ToList();

            var mensajeEntrega = $"📦 Nueva Entrega {entrega.Codigo} del repuesto {repuesto.Codigo} - {repuesto.Nombre}, entregado por {usuario.Nombre}";
            var notiRepo = new NotificacionRepository(connectionString);

            foreach (var mecanico in usuariosMecanicos)
            {
                var noti = new Notificacion
                {
                    UsuarioId = mecanico.Id,
                    Mensaje = mensajeEntrega,
                    Estado = "Nuevo",
                    Fecha = DateTime.Now

                };
                notiRepo.Insertar(noti);
            }

            // Calcular total entregado y actualizar estado de solicitud
            var entregas = entregaRepo.ObtenerPorSolicitudId(entrega.SolicitudId);
            int totalEntregado = 0;
            foreach (var e in entregas)
                totalEntregado += e.CantidadEntregada;

            if (totalEntregado >= solicitud.CantidadSolicitada)
                solicitud.Estado = "Entregada";
            else
                solicitud.Estado = "Pendiente";

            solicitudRepo.Actualizar(solicitud);

            TempData["Mensaje"] = "Entrega registrada correctamente.";
            return RedirectToAction("Index", "Solicitud");
        }

        public ActionResult Index(string filtro)
        {
            var entregas = entregaRepo.ObtenerTodos();

            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.ToLower();
                entregas = entregas.Where(e =>
                    (e.Solicitud?.Codigo?.ToLower().Contains(filtro) ?? false) ||
                    (e.Solicitud?.Repuesto?.Codigo?.ToLower().Contains(filtro) ?? false) ||
                    (e.Solicitud?.Repuesto?.Nombre?.ToLower().Contains(filtro) ?? false)
                ).ToList();
            }

            return View(entregas);
        }
    }
}