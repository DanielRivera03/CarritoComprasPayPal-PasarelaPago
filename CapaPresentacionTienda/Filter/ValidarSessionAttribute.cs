using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacionTienda.Filter
{
    // VALIDAR SI CLIENTE HA INICIADO SESION AL PORTAL DE COMPRAS
    public class ValidarSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (HttpContext.Current.Session["Cliente"] == null)
            {
                filterContext.Result = new RedirectResult("~/Acceso/Index");
                return;
            }
            base.OnActionExecuted(filterContext);
        }
    }
}