using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Proyecto1_Paula_Ulate.Models;

namespace Proyecto1_Paula_Ulate.LogicaDatos
{
    public class EntregaRepository
    {
        private readonly string connectionString;

        public EntregaRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["ConexionBaseDatos"].ConnectionString;
        }

        public void Insertar(Entrega entrega)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string insertQuery = @"
                    INSERT INTO Entrega (FechaEntrega, EntregadoPor, SolicitudId, Observaciones, RecibidoPor, CantidadEntregada)
                    VALUES (@FechaEntrega, @EntregadoPor, @SolicitudId, @Observaciones, @RecibidoPor, @CantidadEntregada);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@FechaEntrega", entrega.FechaEntrega);
                    cmd.Parameters.AddWithValue("@EntregadoPor", entrega.EntregadoPor);
                    cmd.Parameters.AddWithValue("@SolicitudId", entrega.SolicitudId);
                    cmd.Parameters.AddWithValue("@Observaciones", entrega.Observaciones ?? "");
                    cmd.Parameters.AddWithValue("@RecibidoPor", entrega.RecibidoPor ?? "");
                    cmd.Parameters.AddWithValue("@CantidadEntregada", entrega.CantidadEntregada);

                    conn.Open();
                    int nuevoId = Convert.ToInt32(cmd.ExecuteScalar());

                    string codigo = "E-" + nuevoId.ToString("D3");

                    string updateCodigo = "UPDATE Entrega SET Codigo = @Codigo WHERE Id = @Id";
                    cmd.CommandText = updateCodigo;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Codigo", codigo);
                    cmd.Parameters.AddWithValue("@Id", nuevoId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Entrega> ObtenerPorSolicitudId(int solicitudId)
        {
            var lista = new List<Entrega>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT Id, Codigo, FechaEntrega, EntregadoPor, Observaciones, RecibidoPor, CantidadEntregada
            FROM Entrega
            WHERE SolicitudId = @SolicitudId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SolicitudId", solicitudId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var entrega = new Entrega
                            {
                                Id = reader.GetInt32(0),
                                Codigo = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                FechaEntrega = reader.GetDateTime(2),
                                EntregadoPor = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Observaciones = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                RecibidoPor = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                CantidadEntregada = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                SolicitudId = solicitudId
                            };

                            lista.Add(entrega);
                        }
                    }
                }
            }

            return lista;
        }

        public List<Entrega> ObtenerTodos()
        {
            var lista = new List<Entrega>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                E.Id, E.Codigo, E.FechaEntrega, E.EntregadoPor, E.Observaciones, 
                E.RecibidoPor, E.CantidadEntregada, 
                S.Id AS SolicitudId, S.Codigo AS SolicitudCodigo, S.CantidadSolicitada, 
                R.Id AS RepuestoId, R.Codigo AS RepuestoCodigo, R.Nombre AS RepuestoNombre
            FROM Entrega E
            INNER JOIN Solicitud S ON E.SolicitudId = S.Id
            INNER JOIN Repuesto R ON S.RepuestoId = R.Id
        ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var repuesto = new Repuesto
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("RepuestoId")),
                                Codigo = reader.GetString(reader.GetOrdinal("RepuestoCodigo")),
                                Nombre = reader.GetString(reader.GetOrdinal("RepuestoNombre"))
                            };

                            var solicitud = new Solicitud
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("SolicitudId")),
                                Codigo = reader.GetString(reader.GetOrdinal("SolicitudCodigo")),
                                CantidadSolicitada = reader.GetInt32(reader.GetOrdinal("CantidadSolicitada")),
                                Repuesto = repuesto
                            };

                            var entrega = new Entrega
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Codigo = reader.IsDBNull(reader.GetOrdinal("Codigo")) ? "" : reader.GetString(reader.GetOrdinal("Codigo")),
                                FechaEntrega = reader.GetDateTime(reader.GetOrdinal("FechaEntrega")),
                                EntregadoPor = reader.GetString(reader.GetOrdinal("EntregadoPor")),
                                Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? "" : reader.GetString(reader.GetOrdinal("Observaciones")),
                                RecibidoPor = reader.GetString(reader.GetOrdinal("RecibidoPor")),
                                CantidadEntregada = reader.GetInt32(reader.GetOrdinal("CantidadEntregada")),
                                SolicitudId = solicitud.Id,
                                Solicitud = solicitud
                            };

                            lista.Add(entrega);
                        }
                    }
                }
            }

            return lista;
        }
    }
}