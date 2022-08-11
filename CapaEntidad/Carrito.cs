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
    public class Carrito
    {
        public int IdCarrito { get; set; }
        // REFERENCIA AL OBJETO ID CLIENTE DE LA CLASE CLIENTE
        public Cliente oCliente { get; set; }
        // REFERENCIA AL OBJETO ID PRODUCTO DE LA CLASE PRODUCTO
        public Producto oProducto { get; set; }
        public int Cantidad { get; set; }
    }
}
