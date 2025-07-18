using Proyecto1_Paula_Ulate.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.LogicaDatos
{
    public class EntregaRepository
    {
        private readonly string _connectionString;

        public EntregaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Insertar(Entrega entrega)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO Entrega (FechaEntrega, EntregadoPor, SolicitudId, Observaciones, RecibidoPor)
                    VALUES (@FechaEntrega, @EntregadoPor, @SolicitudId, @Observaciones, @RecibidoPor)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FechaEntrega", entrega.FechaEntrega);
                cmd.Parameters.AddWithValue("@EntregadoPor", entrega.EntregadoPor);
                cmd.Parameters.AddWithValue("@SolicitudId", entrega.SolicitudId);
                cmd.Parameters.AddWithValue("@Observaciones", entrega.Observaciones ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RecibidoPor", entrega.RecibidoPor);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Entrega> ObtenerTodas()
        {
            List<Entrega> entregas = new List<Entrega>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Entrega";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    entregas.Add(new Entrega
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        FechaEntrega = Convert.ToDateTime(reader["FechaEntrega"]),
                        EntregadoPor = reader["EntregadoPor"].ToString(),
                        SolicitudId = Convert.ToInt32(reader["SolicitudId"]),
                        Observaciones = reader["Observaciones"] != DBNull.Value ? reader["Observaciones"].ToString() : null,
                        RecibidoPor = reader["RecibidoPor"].ToString()
                    });
                }
            }

            return entregas;
        }

        public Entrega ObtenerPorId(int id)
        {
            Entrega entrega = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Entrega WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    entrega = new Entrega
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        FechaEntrega = Convert.ToDateTime(reader["FechaEntrega"]),
                        EntregadoPor = reader["EntregadoPor"].ToString(),
                        SolicitudId = Convert.ToInt32(reader["SolicitudId"]),
                        Observaciones = reader["Observaciones"] != DBNull.Value ? reader["Observaciones"].ToString() : null,
                        RecibidoPor = reader["RecibidoPor"].ToString()
                    };
                }
            }

            return entrega;
        }

        public void Actualizar(Entrega entrega)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE Entrega
                    SET FechaEntrega = @FechaEntrega,
                        EntregadoPor = @EntregadoPor,
                        SolicitudId = @SolicitudId,
                        Observaciones = @Observaciones,
                        RecibidoPor = @RecibidoPor
                    WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", entrega.Id);
                cmd.Parameters.AddWithValue("@FechaEntrega", entrega.FechaEntrega);
                cmd.Parameters.AddWithValue("@EntregadoPor", entrega.EntregadoPor);
                cmd.Parameters.AddWithValue("@SolicitudId", entrega.SolicitudId);
                cmd.Parameters.AddWithValue("@Observaciones", entrega.Observaciones ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RecibidoPor", entrega.RecibidoPor);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Entrega WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}