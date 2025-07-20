using Proyecto1_Paula_Ulate.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Proyecto1_Paula_Ulate.LogicaDatos
{
    public class UsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Insertar(Usuario usuario)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Obtener el siguiente ID
                string getIdQuery = "SELECT ISNULL(MAX(Id), 0) + 1 FROM Usuario";
                SqlCommand getIdCmd = new SqlCommand(getIdQuery, conn);
                int nextId = (int)getIdCmd.ExecuteScalar();

                string codigoGenerado = "U-" + nextId.ToString("D3");

                string query = @"
            INSERT INTO Usuario (Usuario, Nombre, Correo, Contraseña, Rol, Preferencias, Codigo)
            VALUES (@Usuario,@Nombre, @Correo, @Contraseña, @Rol, @Preferencias, @Codigo)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Usuario", usuario.UsuarioID);
                cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
                cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
                cmd.Parameters.AddWithValue("@Preferencias", (object)usuario.Preferencias ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Codigo", codigoGenerado);

                cmd.ExecuteNonQuery();
            }
        }

        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> usuarios = new List<Usuario>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuario";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    usuarios.Add(new Usuario
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Codigo = reader["Codigo"].ToString(),
                        UsuarioID = reader["Usuario"].ToString(),
                        Nombre = reader["Nombre"].ToString(),
                        Correo = reader["Correo"].ToString(),
                        Contraseña = reader["Contraseña"].ToString(),
                        Rol = reader["Rol"].ToString(),
                        Preferencias = reader["Preferencias"] != DBNull.Value ? reader["Preferencias"].ToString() : null
                    });
                }
            }

            return usuarios;
        }

        public Usuario ObtenerPorId(int id)
        {
            Usuario usuario = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuario WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    usuario = new Usuario
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Codigo = reader["Codigo"].ToString(),
                        UsuarioID = reader["Usuario"].ToString(),
                        Nombre = reader["Nombre"].ToString(),
                        Correo = reader["Correo"].ToString(),
                        Contraseña = reader["Contraseña"].ToString(),
                        Rol = reader["Rol"].ToString(),
                        Preferencias = reader["Preferencias"] != DBNull.Value ? reader["Preferencias"].ToString() : null
                    };
                }
            }

            return usuario;
        }

        public void Actualizar(Usuario usuario)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE Usuario
                    SET Usuario = @Usuario,
                        Nombre = @Nombre,
                        Correo = @Correo,
                        Contraseña = @Contraseña,
                        Rol = @Rol,
                        [Preferencias] = @Preferencias
                    WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", usuario.Id);
                cmd.Parameters.AddWithValue("@usuario", usuario.UsuarioID);
                cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
                cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
                cmd.Parameters.AddWithValue("@Preferencias", usuario.Preferencias);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(string usuarioId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Usuario WHERE UsuarioID = @UsuarioID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UsuarioID", usuarioId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}