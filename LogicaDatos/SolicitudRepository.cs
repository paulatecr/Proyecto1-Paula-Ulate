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
                conn.Open();

                // Paso 1: Insertar solicitud
                string insertQuery = @"
            INSERT INTO Solicitud (Solicitante, FechaSolicitud, RepuestoId, CantidadSolicitada, Estado)
            VALUES (@Solicitante, @FechaSolicitud, @RepuestoId, @CantidadSolicitada, @Estado);
            SELECT SCOPE_IDENTITY();";

                int nuevoId = 0;
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Solicitante", solicitud.Solicitante);
                    cmd.Parameters.AddWithValue("@FechaSolicitud", solicitud.FechaSolicitud);
                    cmd.Parameters.AddWithValue("@RepuestoId", solicitud.RepuestoId);
                    cmd.Parameters.AddWithValue("@CantidadSolicitada", solicitud.CantidadSolicitada);
                    cmd.Parameters.AddWithValue("@Estado", solicitud.Estado);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        nuevoId = Convert.ToInt32(result);
                }

                // Paso 2: Actualizar el Código generado
                string nuevoCodigo = "S-" + nuevoId.ToString("D3");

                string updateQuery = "UPDATE Solicitud SET Codigo = @Codigo WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Codigo", nuevoCodigo);
                    cmd.Parameters.AddWithValue("@Id", nuevoId);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public void Actualizar(Solicitud solicitud)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    UPDATE Solicitud
                    SET Solicitante = @Solicitante,
                        FechaSolicitud = @FechaSolicitud,
                        RepuestoId = @RepuestoId,
                        Codigo = @Codigo,
                        CantidadSolicitada = @CantidadSolicitada,
                        Estado = @Estado
                    WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Solicitante", solicitud.Solicitante);
                    cmd.Parameters.AddWithValue("@Codigo", solicitud.Codigo);
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
                string query = "DELETE FROM Solicitud WHERE Id = @Id";

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
            SELECT s.Id, s.Solicitante, s.Codigo, s.FechaSolicitud, s.RepuestoId, s.CantidadSolicitada, s.Estado,
                   r.Codigo AS RepuestoCodigo, r.Nombre AS RepuestoNombre
            FROM Solicitud s
            JOIN Repuesto r ON s.RepuestoId = r.Id";

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
                                Codigo = reader.GetString(2),
                                FechaSolicitud = reader.GetDateTime(3),
                                RepuestoId = reader.GetInt32(4),
                                CantidadSolicitada = reader.GetInt32(5),
                                Estado = reader.GetString(6),
                                Repuesto = new Repuesto
                                {
                                    Codigo = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                    Nombre = reader.IsDBNull(8) ? "" : reader.GetString(8)
                                },
                                Entregas = new List<Entrega>()
                            };

                            lista.Add(solicitud);
                        }
                    }
                }
            }

            // Cargar entregas asociadas para cada solicitud
            var entregaRepo = new EntregaRepository();
            foreach (var solicitud in lista)
            {
                solicitud.Entregas = entregaRepo.ObtenerPorSolicitudId(solicitud.Id);
            }

            return lista;
        }

        public Solicitud BuscarPorId(int id)
        {
            Solicitud solicitud = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT s.Id, s.Solicitante, s.Codigo, s.FechaSolicitud,
                   s.RepuestoId, s.CantidadSolicitada, s.Estado,
                   r.Nombre AS NombreRepuesto, r.Codigo AS CodigoRepuesto
            FROM Solicitud s
            INNER JOIN Repuesto r ON s.RepuestoId = r.Id
            WHERE s.Id = @Id";

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
                                Codigo = reader.GetString(2),
                                FechaSolicitud = reader.GetDateTime(3),
                                RepuestoId = reader.GetInt32(4),
                                CantidadSolicitada = reader.GetInt32(5),
                                Estado = reader.GetString(6),
                                Repuesto = new Repuesto
                                {
                                    Nombre = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                    Codigo = reader.IsDBNull(8) ? "" : reader.GetString(8)
                                }
                            };
                        }
                    }
                }
            }

            return solicitud;
        }

        public void ActualizarCodigo(int id, string codigo)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Solicitud SET Codigo = @Codigo WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Codigo", codigo);
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}