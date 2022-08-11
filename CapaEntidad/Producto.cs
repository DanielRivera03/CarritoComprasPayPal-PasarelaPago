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

namespace CapaEntidad
{
	public class Producto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
		// REFERENCIA AL OBJETO ID MARCA DE LA CLASE MARCA
        public Marca oMarca { get; set; }
		// REFERENCIA AL OBJETO ID CATEGORIA DE LA CLASE CATEGORIA
		public Categoria oCategoria { get; set; }
		public decimal Precio { get; set; }
		public string PrecioTexto { get; set; }
		public int Stock { get; set; }
		public string RutaImagen { get; set; }
		public string NombreImagen { get; set; }
		public bool Activo { get; set; }
		public string Base64 { get; set; }
		public string Extension { get; set; }

	}
}
