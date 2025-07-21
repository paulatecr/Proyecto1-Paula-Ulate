using Proyecto1_Paula_Ulate.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Proyecto1_Paula_Ulate.LogicaDatos
{
    public class RepuestoRepository
    {
        private readonly string _connectionString;

        public RepuestoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Repuesto> ObtenerTodos()
        {
            var lista = new List<Repuesto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Repuesto";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Repuesto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Codigo = reader["Codigo"].ToString(),
                        Nombre = reader["Nombre"].ToString(),
                        Descripcion = reader["Descripcion"].ToString(),
                        CantidadDisponible = Convert.ToInt32(reader["CantidadDisponible"]),
                        PrecioUnitario = Convert.ToDecimal(reader["PrecioUnitario"])
                    });
                }
            }

            return lista;
        }

        public void Insertar(Repuesto repuesto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // 1. Insertar repuesto SIN Código
                string insertQuery = @"
            INSERT INTO Repuesto (Nombre, Descripcion, CantidadDisponible, PrecioUnitario)
            OUTPUT INSERTED.Id
            VALUES (@Nombre, @Descripcion, @CantidadDisponible, @PrecioUnitario)";

                SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@Nombre", repuesto.Nombre);
                insertCmd.Parameters.AddWithValue("@Descripcion", repuesto.Descripcion);
                insertCmd.Parameters.AddWithValue("@CantidadDisponible", repuesto.CantidadDisponible);
                insertCmd.Parameters.AddWithValue("@PrecioUnitario", repuesto.PrecioUnitario);

                int nuevoId = (int)insertCmd.ExecuteScalar();

                // 2. Generar Código
                string codigoGenerado = "R-" + nuevoId.ToString("D3");

                // 3. Actualizar con el nuevo Código
                string updateQuery = "UPDATE Repuesto SET Codigo = @Codigo WHERE Id = @Id";
                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@Codigo", codigoGenerado);
                updateCmd.Parameters.AddWithValue("@Id", nuevoId);
                updateCmd.ExecuteNonQuery();
            }
        }

        public Repuesto ObtenerPorId(int id)
        {
            Repuesto repuesto = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Repuesto WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    repuesto = new Repuesto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Codigo = reader["Codigo"].ToString(),
                        Nombre = reader["Nombre"].ToString(),
                        Descripcion = reader["Descripcion"].ToString(),
                        CantidadDisponible = Convert.ToInt32(reader["CantidadDisponible"]),
                        PrecioUnitario = Convert.ToDecimal(reader["PrecioUnitario"])
                    };
                }
            }

            return repuesto;
        }

        public void Actualizar(Repuesto repuesto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE Repuesto
                    SET Nombre = @Nombre,
                        Descripcion = @Descripcion,
                        CantidadDisponible = @CantidadDisponible,
                        PrecioUnitario = @PrecioUnitario
                    WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", repuesto.Id);
                cmd.Parameters.AddWithValue("@Codigo", repuesto.Codigo);
                cmd.Parameters.AddWithValue("@Nombre", repuesto.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", repuesto.Descripcion);
                cmd.Parameters.AddWithValue("@CantidadDisponible", repuesto.CantidadDisponible);
                cmd.Parameters.AddWithValue("@PrecioUnitario", repuesto.PrecioUnitario);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void ActualizarCantidad(Repuesto repuesto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Repuesto SET CantidadDisponible = @Cantidad WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Cantidad", repuesto.CantidadDisponible);
                cmd.Parameters.AddWithValue("@Id", repuesto.Id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Repuesto WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}