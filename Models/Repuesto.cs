using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.Models
{
    public class Repuesto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
    }
}