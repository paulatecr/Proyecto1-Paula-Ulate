using Proyecto1_Paula_Ulate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Proyecto1_Paula_Ulate.LogicaDatos
{
    public class NotificacionRepository
    {
        private readonly string connectionString;
        public NotificacionRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Insertar(Notificacion notificacion)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    INSERT INTO Notificacion (UsuarioId, Mensaje, Estado, Fecha)
                    VALUES (@UsuarioId, @Mensaje, @Estado, @Fecha)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UsuarioId", notificacion.UsuarioId);
                    cmd.Parameters.AddWithValue("@Mensaje", notificacion.Mensaje);
                    cmd.Parameters.AddWithValue("@Estado", notificacion.Estado);
                    cmd.Parameters.AddWithValue("@Fecha", notificacion.Fecha < new DateTime(1753, 1, 1)
                    ? DateTime.Now
                    : notificacion.Fecha);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Notificacion> ObtenerPorUsuario(int usuarioId)
        {
            var lista = new List<Notificacion>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT Id, UsuarioId, Mensaje, Estado, Fecha
                    FROM Notificacion
                    WHERE UsuarioId = @UsuarioId
                    ORDER BY Fecha DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Notificacion
                            {
                                Id = reader.GetInt32(0),
                                UsuarioId = reader.GetInt32(1),
                                Mensaje = reader.GetString(2),
                                Estado = reader.GetString(3),
                                Fecha = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public void MarcarComoLeida(int notificacionId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Notificacion SET Estado = 'Leída' WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", notificacionId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}