using System;

namespace Proyecto1_Paula_Ulate.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string UsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string Rol { get; set; }
        public string Preferencias { get; set; }

        // Propiedad auxiliar para el tema (puede leer y escribir)
        public string Tema
        {
            get
            {
                if (string.IsNullOrEmpty(Preferencias)) return "claro";
                try
                {
                    dynamic prefs = Newtonsoft.Json.JsonConvert.DeserializeObject(Preferencias);
                    return prefs.tema ?? "claro";
                }
                catch { return "claro"; }
            }
            set
            {
                Preferencias = ActualizarPreferencias(value, TamañoLetra);
            }
        }

        // Propiedad auxiliar para tamaño de letra (puede leer y escribir)
        public string TamañoLetra
        {
            get
            {
                if (string.IsNullOrEmpty(Preferencias)) return "normal";
                try
                {
                    dynamic prefs = Newtonsoft.Json.JsonConvert.DeserializeObject(Preferencias);
                    return prefs.tamañoLetra ?? "normal";
                }
                catch { return "normal"; }
            }
            set
            {
                Preferencias = ActualizarPreferencias(Tema, value);
            }
        }

        // Método interno para actualizar el JSON completo de preferencias
        private string ActualizarPreferencias(string tema, string tamanioLetra)
        {
            var obj = new
            {
                tema = tema ?? "claro",
                tamañoLetra = tamanioLetra ?? "normal"
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }
}