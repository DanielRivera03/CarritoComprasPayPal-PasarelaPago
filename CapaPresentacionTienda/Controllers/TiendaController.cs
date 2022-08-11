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
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CapaEntidad;
using CapaNegocio;
using CapaEntidad.Paypal;
// VALIDACION DE SESIONES
using CapaPresentacionTienda.Filter;

namespace CapaPresentacionTienda.Controllers
{
    public class TiendaController : Controller
    {
        // GET: Tienda
        public ActionResult Index()
        {
            return View();
        }

        // VISTA DETALLES DE PRODUCTOS
        public ActionResult DetalleProducto(int idproducto = 0)
        {
            Producto oProducto = new Producto();
            bool conversion;
            oProducto = new CN_Producto().Listar().Where(p => p.IdProducto == idproducto).FirstOrDefault();
            if (oProducto != null)
            {
                oProducto.Base64 = CN_Recursos.ConvertirBase64(Path.Combine(oProducto.RutaImagen, oProducto.NombreImagen),out conversion);
                oProducto.Extension = Path.GetExtension(oProducto.NombreImagen);
            }

            return View(oProducto);
        }

        // LISTAR TODAS LAS CATEGORIAS
        [HttpGet]
        public JsonResult ListaCategorias()
        {
            List<Categoria> lista  = new List<Categoria>();
            lista = new CN_Categoria().Listar();
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);

        }
        // LISTAR MARCAS POR CATEGORIA
        [HttpPost]
        public JsonResult ListaMarcaPorCategoria(int idcategoria)
        {
            List<Marca> lista = new List<Marca>();
            lista = new CN_Marca().ListarMarcaPorCategoria(idcategoria);
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);

        }
        // LISTAR TODOS LOS PRODUCTOS A MOSTRAR EN TIENDA
        [HttpPost]
        public JsonResult ListarProductos(int idcategoria, int idmarca)
        {
            List<Producto> lista = new List<Producto>();
            bool conversion;
            lista = new CN_Producto().Listar().Select(p => new Producto()
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                oMarca = p.oMarca,
                oCategoria = p.oCategoria,
                Precio = p.Precio,
                Stock = p.Stock,
                RutaImagen = p.RutaImagen,
                Base64 = CN_Recursos.ConvertirBase64(Path.Combine(p.RutaImagen, p.NombreImagen), out conversion),
                Extension = Path.GetExtension(p.NombreImagen),
                Activo = p.Activo
            }).Where(p =>
                p.oCategoria.IdCategoria == (idcategoria == 0 ? p.oCategoria.IdCategoria : idcategoria) &&
                p.oMarca.IdMarca == (idmarca == 0 ? p.oMarca.IdMarca : idmarca) &&
                p.Stock > 0 && p.Activo == true
                ).ToList();
            var jsonresult = Json(new { data = lista }, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = int.MaxValue;
            return jsonresult;
        }

        // AGREGAR PRODUCTOS AL CARRITO
        [HttpPost]
        public JsonResult AgregarCarrito(int idproducto)
        {
            int idcliente = ((Cliente)Session["Cliente"]).IdCliente; // CONVERTIR LA VARIABLE DE SESION EN UN OBJETO CLIENTE
            bool existe = new CN_Carrito().ExisteCarrito(idcliente, idproducto);
            bool respuesta = false;
            string mensaje = string.Empty;
            if (existe)
            {
                mensaje = "Lo sentimos, el producto ya existe en el carrito de compras";
            }
            else
            {
                respuesta = new CN_Carrito().OperacionCarrito(idcliente, idproducto, true, out mensaje);
            }
            return Json(new { respuesta = respuesta , mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        // DEVOLVER CANTIDAD DE PRODUCTOS AGREGADOS AL CARRITO DE COMPRAS
        [HttpGet]
        public JsonResult CantidadEnCarrito()
        {
            int idcliente = ((Cliente)Session["Cliente"]).IdCliente; // CONVERTIR LA VARIABLE DE SESION EN UN OBJETO CLIENTE
            int cantidad = new CN_Carrito().CantidadEnCarrito(idcliente);
            return Json(new { cantidad = cantidad }, JsonRequestBehavior.AllowGet);
        }
        // LISTAR TODOS LOS PRODUCTOS AGREGADOS AL CARRITO DE COMPRAS CLIENTES
        [HttpPost]
        public JsonResult ListarProductosCarrito()
        {
            int idcliente = ((Cliente)Session["Cliente"]).IdCliente; // CONVERTIR LA VARIABLE DE SESION EN UN OBJETO CLIENTE
            List<Carrito> oLista = new List<Carrito>();
            bool conversion;
            oLista = new CN_Carrito().ListarProducto(idcliente).Select(listacompra => new Carrito()
            {
                oProducto = new Producto()
                {
                    IdProducto = listacompra.oProducto.IdProducto,
                    Nombre = listacompra.oProducto.Nombre,
                    oMarca = listacompra.oProducto.oMarca,
                    Precio = listacompra.oProducto.Precio,
                    RutaImagen = listacompra.oProducto.RutaImagen,
                    Base64 = CN_Recursos.ConvertirBase64(Path.Combine(listacompra.oProducto.RutaImagen, listacompra.oProducto.NombreImagen), out conversion), Extension = Path.GetExtension(listacompra.oProducto.NombreImagen)
                },
                Cantidad = listacompra.Cantidad
            }).ToList();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        // GESTIONAR TODAS LAS OPERACIONES DE CARRITO DE COMPRAS CLIENTES
        [HttpPost]
        public JsonResult OperacionCarrito(int idproducto, bool sumar)
        {
            int idcliente = ((Cliente)Session["Cliente"]).IdCliente; // CONVERTIR LA VARIABLE DE SESION EN UN OBJETO CLIENTE
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Carrito().OperacionCarrito(idcliente, idproducto, sumar, out mensaje);
            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        // ELIMINAR PRODUCTOS AGREGADOS EN CARRITO DE COMPRAS
        [HttpPost]
        public JsonResult EliminarCarrito(int idproducto) 
        {
            int idcliente = ((Cliente)Session["Cliente"]).IdCliente; // CONVERTIR LA VARIABLE DE SESION EN UN OBJETO CLIENTE   
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Carrito().EliminarCarrito(idcliente, idproducto);
            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        // OBTENER LISTADO DE DEPARTAMENTOS
        [HttpPost]
        public JsonResult ObtenerDepartamento()
        {
            List<Departamento> oLista = new List<Departamento>();
            oLista = new CN_Ubicacion().ObtenerDepartamento();
            return Json(new { lista = oLista }, JsonRequestBehavior.AllowGet);
        }
        // OBTENER LISTADO DE MUNICIPIOS
        [HttpPost]
        public JsonResult ObtenerMunicipio(string iddepartamento)
        {
            List<Municipio> oLista = new List<Municipio>();
            oLista = new CN_Ubicacion().ObtenerMunicipio(iddepartamento);
            return Json(new { lista = oLista }, JsonRequestBehavior.AllowGet);
        }
        [ValidarSession]
        [Authorize]
        public ActionResult Carrito()
        {
            return View();
        }
        // PROCESAR PAGO Y FINALIZAR VENTA DE PRODUCTOS CARRITO DE COMPRAS
        [HttpPost]
        public async Task<JsonResult> ProcesarPago(List<Carrito> oListaCarrito, Venta oVenta)
        {
            decimal total = 0;
            DataTable detalle_venta = new DataTable();
            detalle_venta.Locale = new CultureInfo("es-SV");
            detalle_venta.Columns.Add("IdProducto", typeof(string));
            detalle_venta.Columns.Add("Cantidad", typeof(int));
            detalle_venta.Columns.Add("Total", typeof(decimal));
            // LISTA DE PRODUCTOS PARA PROCESAMIENTO DATOS PAYPAL
            List<Item> oListaItem = new List<Item>();
            foreach(Carrito oCarrito in oListaCarrito)
            {
                decimal subtotal = Convert.ToDecimal(oCarrito.Cantidad.ToString()) * oCarrito.oProducto.Precio;
                total += subtotal;
                oListaItem.Add(new Item()
                {
                    name = oCarrito.oProducto.Nombre,
                    quantity = oCarrito.Cantidad.ToString(),
                    unit_amount = new UnitAmount()
                    {
                        currency_code = "USD",
                        value = oCarrito.oProducto.Precio.ToString("G", new CultureInfo("es-SV"))
                    }
                });
                detalle_venta.Rows.Add(new object[]
                {
                    oCarrito.oProducto.IdProducto,
                    oCarrito.Cantidad,
                    subtotal
                });
            }
            PurchaseUnit purchaseUnit = new PurchaseUnit()
            {
                amount = new Amount()
                {
                    currency_code = "USD",
                    value = total.ToString("G", new CultureInfo("es-SV")),
                    breakdown = new Breakdown()
                    {
                        item_total = new ItemTotal()
                        {
                            currency_code = "USD",
                            value = total.ToString("G", new CultureInfo("es-SV")),
                        }
                    }
                },
                description = "Compra Online Articulos Mi Tienda Comercial En Linea",
                items = oListaItem
            };

            Checkout_Order oCheckOutOrder = new Checkout_Order()
            {
                intent = "CAPTURE",
                purchase_units = new List<PurchaseUnit>() { purchaseUnit },
                application_context = new ApplicationContext()
                {
                    brand_name = "MIURL.COM",
                    landing_page = "NO_PREFERENCE",
                    user_action = "PAY_NOW",
                    // CAMBIAR POR URL DE SU SERVIDOR
                    return_url = "xxxxxxx/Tienda/PagoEfectuado",
                    cancel_url = "xxxxxxx/Tienda/Carrito"
                }
            };
            oVenta.MontoTotal = total;
            oVenta.IdCliente = ((Cliente)Session["Cliente"]).IdCliente;
            TempData["Venta"] = oVenta;
            TempData["DetalleVenta"] = detalle_venta;
            // EJECUTAR LOS SERVICIOS DE PAYPAL
            CN_Paypal opaypal = new CN_Paypal();
            Response_Paypal<Response_Checkout> response_paypal = new Response_Paypal<Response_Checkout>();
            response_paypal = await opaypal.CrearSolicitud(oCheckOutOrder);
            return Json(response_paypal,JsonRequestBehavior.AllowGet);
        }

        [ValidarSession]
        [Authorize]
        // CONFIRMACION DE COMPRA
        public async Task<ActionResult> Pagoefectuado()
        {
            string token = Request.QueryString["token"];
            CN_Paypal opaypal = new CN_Paypal();
            Response_Paypal<Response_Capture> response_paypal = new Response_Paypal<Response_Capture>();
            response_paypal = await opaypal.AprobarPago(token);

            bool status = Convert.ToBoolean(Request.QueryString["status"]);
            ViewData["Status"] = response_paypal.Status;
            if (response_paypal.Status)
            {
                Venta oVenta = (Venta)TempData["Venta"];
                DataTable detalle_venta = (DataTable)TempData["DetalleVenta"];
                oVenta.IdTransaccion = response_paypal.Response.purchase_units[0].payments.captures[0].id;
                string mensaje = string.Empty;
                bool respuesta = new CN_Venta().Registrar(oVenta,detalle_venta,out mensaje);
                ViewData["IdTransaccion"] = oVenta.IdTransaccion;
            }
            return View();
        }

        // LISTAR TODOS LOS PRODUCTOS COMPRADOS POR CLIENTES [VISTA]
        [ValidarSession]
        [Authorize]
        public ActionResult MisCompras()
        {
            int idcliente = ((Cliente)Session["Cliente"]).IdCliente; // CONVERTIR LA VARIABLE DE SESION EN UN OBJETO CLIENTE
            List<DetalleVenta> oLista = new List<DetalleVenta>();
            bool conversion;
            oLista = new CN_Venta().ListarCompras(idcliente).Select(listacompra => new DetalleVenta()
            {
                oProducto = new Producto()
                {
                    Nombre = listacompra.oProducto.Nombre,
                    Precio = listacompra.oProducto.Precio,
                    Base64 = CN_Recursos.ConvertirBase64(Path.Combine(listacompra.oProducto.RutaImagen, listacompra.oProducto.NombreImagen), out conversion),
                    Extension = Path.GetExtension(listacompra.oProducto.NombreImagen)
                },
                Cantidad = listacompra.Cantidad,
                Total = listacompra.Total,
                IdTransaccion = listacompra.IdTransaccion
            }).ToList();
            return View(oLista);
        }

    }
}
