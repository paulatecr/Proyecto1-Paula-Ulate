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
    }
}