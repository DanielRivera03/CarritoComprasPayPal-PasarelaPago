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
using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CapaPresentacionTienda.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Registrar()
        {
            return View();
        }
        public ActionResult Reestablecer()
        {
            return View();
        }
        public ActionResult CambiarClave()
        {
            return View();
        }

        [HttpPost]
        // REGISTRAR NUEVOS CLIENTES
        public ActionResult Registrar(Cliente objeto)
        {
            int resultado;
            string mensaje = string.Empty;
            ViewData["Nombres"] = string.IsNullOrEmpty(objeto.Nombres) ? "" : objeto.Nombres;
            ViewData["Apellidos"] = string.IsNullOrEmpty(objeto.Apellidos) ? "" : objeto.Apellidos;
            ViewData["Correo"] = string.IsNullOrEmpty(objeto.Correo) ? "" : objeto.Correo;
            if (objeto.Clave != objeto.ConfirmarClave)
            {
                ViewBag.Error = "Lo sentimos, las contraseñas no coinciden";
                return View();
            }
            resultado = new CN_Cliente().Registrar(objeto, out mensaje);
            if (resultado > 0)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "Acceso");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }
        }

        // INICIO DE SESION CLIENTES
        [HttpPost]
        public ActionResult Index(string correo, string clave)
        {
            Cliente oCliente = null;
            oCliente = new CN_Cliente().Listar().Where(cliente => cliente.Correo == correo && cliente.Clave == CN_Recursos.ConvertirSha256(clave)).FirstOrDefault();
            if (oCliente == null)
            {
                ViewBag.Error = "Lo sentimos, el correo y/o contraseña no son correctas";
                return View();
            }
            else
            {
                // SI CLIENTE NECESITA REESTABLECER CLAVE, REDIRIGIR A ESA VISTA
                if (oCliente.Reestablecer)
                {
                    TempData["IdCliente"] = oCliente.IdCliente;
                    return RedirectToAction("CambiarClave", "Acceso");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(oCliente.Correo, false);
                    Session["Cliente"] = oCliente;
                    ViewBag.Error = null;
                    return RedirectToAction("Index", "Tienda");

                }
            }
        }

        // REESTABLECER CLAVE CLIENTES [RECUPERACION CUENTAS]
        [HttpPost]
        public ActionResult Reestablecer(string correo)
        {
            Cliente cliente = new Cliente();
            cliente = new CN_Cliente().Listar().Where(usuario => usuario.Correo == correo).FirstOrDefault();
            if (cliente == null)
            {
                ViewBag.Error = "Lo sentimos, no se ha encontrado un cliente relacionado a ese correo";
                return View();
            }
            else
            {
                string mensaje = string.Empty;
                bool respuesta = new CN_Cliente().ReestablecerClave(cliente.IdCliente, correo, out mensaje);
                if (respuesta)
                {
                    ViewBag.Error = null;
                    ViewBag.Success = "Por favor revise su correo y siga las instrucciones ahí descritas";
                    //return RedirectToAction("Index", "Acceso");
                    return View();
                }
                else
                {
                    ViewBag.Error = mensaje;
                    return View();
                }
            }
        }

        // CAMBIAR CLAVE DE CLIENTES
        [HttpPost]
        public ActionResult CambiarClave(string idcliente, string claveactual, string nuevaclave, string confirmarclave)
        {
            Cliente oCliente = new Cliente();
            oCliente = new CN_Cliente().Listar().Where(cliente => cliente.IdCliente == int.Parse(idcliente)).FirstOrDefault();
            // VERIFICAR SI CLAVE ES IGUAL A LA SOLICITADA
            if (oCliente.Clave != CN_Recursos.ConvertirSha256(claveactual))
            {
                TempData["IdCliente"] = idcliente;
                ViewData["vclave"] = "";
                ViewBag.Error = "Lo sentimos, su contraseña actual no es correcta";
                return View();
            }
            // VERIFICAR SI CLAVE NUEVA ES IGUAL A LA CLAVE DE REPLICACION
            else if (nuevaclave != confirmarclave)
            {
                TempData["IdCliente"] = idcliente;
                ViewData["vclave"] = claveactual;
                ViewBag.Error = "Lo sentimos, las contraseñas ingresadas no son iguales";
                return View();
            }
            // REALIZAR EJECUCION DE NUEVA CLAVE USUARIOS
            ViewData["vclave"] = "";
            nuevaclave = CN_Recursos.ConvertirSha256(nuevaclave);
            string mensaje = string.Empty;
            bool respuesta = new CN_Cliente().CambiarClave(int.Parse(idcliente), nuevaclave, out mensaje);
            if (respuesta)
            {
                return RedirectToAction("Index");
                
            }
            else
            {
                TempData["IdCliente"] = idcliente;
                ViewBag.Error = mensaje;
                return View();
            }
        }

        // CERRAR SESIONES DE CLIENTES
        public ActionResult CerrarSesion()
        {
            Session["Cliente"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Acceso");
        }


    }
}