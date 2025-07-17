using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.Models
{
    public class Solicitud
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Solicitante { get; set; }

        [Required]
        public DateTime FechaSolicitud { get; set; }

        // Relación con Repuesto
        [ForeignKey("Repuesto")]
        public int RepuestoId { get; set; }
        public Repuesto Repuesto { get; set; }

        [Required]
        public int CantidadSolicitada { get; set; }

        public string Estado { get; set; } // Ej: Pendiente, Aprobada, Rechazada
    }
}