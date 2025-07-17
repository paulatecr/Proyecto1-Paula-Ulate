using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.Models
{
    public class BodegaContext : DbContext
    {
        public BodegaContext() : base("BodegaContext") { }

        public DbSet<Repuesto> Repuestos { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<Entrega> Entregas { get; set; }
    }
}