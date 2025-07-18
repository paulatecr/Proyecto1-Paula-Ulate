using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.Models
{
    public class Entrega
    {
        public int Id { get; set; }
        public DateTime FechaEntrega { get; set; }

        public string EntregadoPor { get; set; }

        // Relación con Solicitud
        public int SolicitudId { get; set; }
        public Solicitud Solicitud { get; set; }

        public string Observaciones { get; set; }
    }
}