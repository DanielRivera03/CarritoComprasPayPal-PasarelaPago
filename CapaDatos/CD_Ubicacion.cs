/*

░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
░        CARRITO DE COMPRAS / MVC ASP.NET ADO.NET          ░
░--------------------------------------------------------- ░
░- -> IDEA ORIGINAL POR PARTE DE [ CODIGO ESTUDIANTE ]     ░
░--------------------------------------------------------- ░
░- -> ADECUADO Y MODIFICADO POR DANIEL RIVERA              ░
░--------------------------------------------------------- ░
░- -> C# - ASP.NET / ADO.NET => .NET FRAMEWORK 4.7 up+     ░
░- -> GITHUB: (danielrivera03)                             ░
░     https://github.com/DanielRivera03                    ░
░--------------------------------------------------------- ░
░- -> TODOS LOS DERECHOS RESERVADOS © 2022                 ░
░                                                          ░
░        ♥♥ HECHO CON ALGUNAS TAZAS DE CAFE ♥♥             ░
░                                                          ░
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
-> POR FAVOR REALIZAR TODAS LAS ADECUACIONES PERTINENTES
       
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Ubicacion
    {
        public List<Departamento> ObtenerDepartamento()
        {
            List<Departamento> lista = new List<Departamento>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    // EJECUTA SENTENCIA SQL USUARIOS
                    string query = "SELECT * FROM DEPARTAMENTO";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    // REALIZA LECTURA DE DATOS
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // LLENADO DE LISTA SEGUN DATOS COINCIDENTES
                            lista.Add(
                                    new Departamento()
                                    {
                                        IdDepartamento = dr["IdDepartamento"].ToString(),
                                        Descripcion = dr["Descripcion"].ToString()
                                    }
                                );
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                lista = new List<Departamento>();
            }
            return lista;
        }

        public List<Municipio> ObtenerMunicipio(string iddepartamento)
        {
            List<Municipio> lista = new List<Municipio>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    // EJECUTA SENTENCIA SQL USUARIOS
                    string query = "SELECT * FROM MUNICIPIO WHERE IdDepartamento = @iddepartamento";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@iddepartamento", iddepartamento);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    // REALIZA LECTURA DE DATOS
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // LLENADO DE LISTA SEGUN DATOS COINCIDENTES
                            lista.Add(
                                    new Municipio()
                                    {
                                        IdProvincia = dr["IdProvincia"].ToString(),
                                        Descripcion = dr["Descripcion"].ToString()
                                    }
                                );
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                lista = new List<Municipio>();
            }
            return lista;
        }


    }
}
