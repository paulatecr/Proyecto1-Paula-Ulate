﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string Rol { get; set; }
    }
}