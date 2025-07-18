using Proyecto1_Paula_Ulate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.LogicaDatos
{
    public class SolicitudRepository
    {
        private readonly string connectionString;

        public SolicitudRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["ConexionBaseDatos"].ConnectionString;
        }

        public void Insertar(Solicitud solicitud)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    INSERT INTO Solicitudes (Solicitante, FechaSolicitud, RepuestoId, CantidadSolicitada, Estado)
                    VALUES (@Solicitante, @FechaSolicitud, @RepuestoId, @CantidadSolicitada, @Estado)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Solicitante", solicitud.Solicitante);
                    cmd.Parameters.AddWithValue("@FechaSolicitud", solicitud.FechaSolicitud);
                    cmd.Parameters.AddWithValue("@RepuestoId", solicitud.RepuestoId);
                    cmd.Parameters.AddWithValue("@CantidadSolicitada", solicitud.CantidadSolicitada);
                    cmd.Parameters.AddWithValue("@Estado", solicitud.Estado);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Solicitud solicitud)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    UPDATE Solicitudes
                    SET Solicitante = @Solicitante,
                        FechaSolicitud = @FechaSolicitud,
                        RepuestoId = @RepuestoId,
                        CantidadSolicitada = @CantidadSolicitada,
                        Estado = @Estado
                    WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Solicitante", solicitud.Solicitante);
                    cmd.Parameters.AddWithValue("@FechaSolicitud", solicitud.FechaSolicitud);
                    cmd.Parameters.AddWithValue("@RepuestoId", solicitud.RepuestoId);
                    cmd.Parameters.AddWithValue("@CantidadSolicitada", solicitud.CantidadSolicitada);
                    cmd.Parameters.AddWithValue("@Estado", solicitud.Estado);
                    cmd.Parameters.AddWithValue("@Id", solicitud.Id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Solicitudes WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Solicitud> ObtenerTodos()
        {
            var lista = new List<Solicitud>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT Id, Solicitante, FechaSolicitud, RepuestoId, CantidadSolicitada, Estado
                    FROM Solicitudes";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var solicitud = new Solicitud
                            {
                                Id = reader.GetInt32(0),
                                Solicitante = reader.GetString(1),
                                FechaSolicitud = reader.GetDateTime(2),
                                RepuestoId = reader.GetInt32(3),
                                CantidadSolicitada = reader.GetInt32(4),
                                Estado = reader.GetString(5)
                            };
                            lista.Add(solicitud);
                        }
                    }
                }
            }
            return lista;
        }

        public Solicitud BuscarPorId(int id)
        {
            Solicitud solicitud = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT Id, Solicitante, FechaSolicitud, RepuestoId, CantidadSolicitada, Estado
                    FROM Solicitudes
                    WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            solicitud = new Solicitud
                            {
                                Id = reader.GetInt32(0),
                                Solicitante = reader.GetString(1),
                                FechaSolicitud = reader.GetDateTime(2),
                                RepuestoId = reader.GetInt32(3),
                                CantidadSolicitada = reader.GetInt32(4),
                                Estado = reader.GetString(5)
                            };
                        }
                    }
                }
            }
            return solicitud;
        }
    }
}