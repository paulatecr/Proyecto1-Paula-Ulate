using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.Models
{
    public class Solicitud
    {

        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Solicitante { get; set; }

        public DateTime FechaSolicitud { get; set; }

        // Relación con Repuesto
        public int RepuestoId { get; set; }
        public Repuesto Repuesto { get; set; }

        public int CantidadSolicitada { get; set; }

        public string Estado { get; set; }
    }
}