using Proyecto1_Paula_Ulate.LogicaDatos;
using Proyecto1_Paula_Ulate.Models;
using System.Web.Mvc;

namespace Proyecto1_Paula_Ulate.Controllers
{
    public class NotificacionController : Controller
    {
        private readonly string connectionString;
        private readonly NotificacionRepository notiRepo;

        public NotificacionController()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConexionBaseDatos"].ConnectionString;
            notiRepo = new NotificacionRepository(connectionString);
        }

        // GET: Notificacion
        public ActionResult Index()
        {
            var usuario = Session["UsuarioLogueado"] as Usuario;
            if (usuario == null)
                return RedirectToAction("Index", "Home");

            var notificaciones = notiRepo.ObtenerPorUsuario(usuario.Id);
            return View("~/Views/Notificacion/Index.cshtml", notificaciones);
        }

        [HttpGet]
        public ActionResult MarcarComoLeida(int id)
        {
            notiRepo.MarcarComoLeida(id);
            return RedirectToAction("Index");
        }
    }
}