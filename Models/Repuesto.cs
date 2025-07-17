using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.Models
{
    public class Repuesto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public string Ubicacion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        public DateTime FechaIngreso { get; set; }
    }
}