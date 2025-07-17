using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.Models
{
    public class Entrega
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaEntrega { get; set; }

        [Required]
        public string EntregadoPor { get; set; }

        // Relación con Solicitud
        [ForeignKey("Solicitud")]
        public int SolicitudId { get; set; }
        public Solicitud Solicitud { get; set; }

        public string Observaciones { get; set; }
    }
}