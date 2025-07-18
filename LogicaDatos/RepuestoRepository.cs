using Proyecto1_Paula_Ulate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.LogicaDatos
{
    public class RepuestoRepository
    {
        private readonly string connectionString;

        public RepuestoRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["ConexionBaseDatos"].ConnectionString;
        }

        public void Insertar(Repuesto repuesto)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Repuestos (Nombre, Descripcion, CantidadDisponible, PrecioUnitario)
                                 VALUES (@Nombre, @Descripcion, @CantidadDisponible, @PrecioUnitario)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", repuesto.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", repuesto.Descripcion);
                    cmd.Parameters.AddWithValue("@CantidadDisponible", repuesto.CantidadDisponible);
                    cmd.Parameters.AddWithValue("@PrecioUnitario", repuesto.PrecioUnitario);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Repuesto repuesto)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Repuestos 
                                 SET Nombre = @Nombre, Descripcion = @Descripcion, CantidadDisponible = @CantidadDisponible, PrecioUnitario = @PrecioUnitario
                                 WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", repuesto.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", repuesto.Descripcion);
                    cmd.Parameters.AddWithValue("@CantidadDisponible", repuesto.CantidadDisponible);
                    cmd.Parameters.AddWithValue("@PrecioUnitario", repuesto.PrecioUnitario);
                    cmd.Parameters.AddWithValue("@Id", repuesto.Id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Repuestos WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Repuesto> ObtenerTodos()
        {
            var lista = new List<Repuesto>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Nombre, Descripcion, CantidadDisponible, PrecioUnitario FROM Repuestos";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var repuesto = new Repuesto
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                CantidadDisponible = reader.GetInt32(3),
                                PrecioUnitario = reader.GetDecimal(4)
                            };
                            lista.Add(repuesto);
                        }
                    }
                }
            }
            return lista;
        }

        public Repuesto BuscarPorId(int id)
        {
            Repuesto repuesto = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Nombre, Descripcion, CantidadDisponible, PrecioUnitario FROM Repuestos WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            repuesto = new Repuesto
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                CantidadDisponible = reader.GetInt32(3),
                                PrecioUnitario = reader.GetDecimal(4)
                            };
                        }
                    }
                }
            }
            return repuesto;
        }
    }
}