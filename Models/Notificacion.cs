using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.Models
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; } // Usuario destinatario
        public string Mensaje { get; set; }
        public string Estado { get; set; } // "Nuevo" o "Leído"
        public DateTime Fecha { get; set; }
    }
}