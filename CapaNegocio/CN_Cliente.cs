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
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Cliente
    {
        // ACCEDIENDO A TODOS LOS DATOS DE LA CAPA DATOS DE CLIENTES
        private CD_Cliente objCapaDato = new CD_Cliente();

        public List<Cliente> Listar()
        {
            return objCapaDato.Listar();
        }

        // REGISTRO DE NUEVOS CLIENTES
        public int Registrar(Cliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Nombres) || string.IsNullOrWhiteSpace(obj.Nombres))
            {
                Mensaje = "El nombre del cliente no puede estar vacio";
            }
            else if (string.IsNullOrEmpty(obj.Apellidos) || string.IsNullOrWhiteSpace(obj.Apellidos))
            {
                Mensaje = "El apellido del cliente no puede estar vacio";
            }
            else if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "El correo del cliente no puede estar vacio";
            }
            if (string.IsNullOrEmpty(Mensaje))
            {
                obj.Clave = CN_Recursos.ConvertirSha256(obj.Clave);
                return objCapaDato.Registrar(obj, out Mensaje);
            } // CIERRE if (string.IsNullOrEmpty(Mensaje))
            else
            {
                return 0;
            }
        }

        // CAMBIAR CLAVE Cliente
        public bool CambiarClave(int idcliente, string nuevaclave, out string mensaje)
        {
            return objCapaDato.CambiarClave(idcliente, nuevaclave, out mensaje);
        }

        // REESTABLECER CLAVE Cliente

       public bool ReestablecerClave(int idcliente, string correo, out string Mensaje)
       {
            Mensaje = string.Empty;
            // GENERAR CLAVE DE ACCESO ALEATORIA
            string nuevaclave = CN_Recursos.GenerarClave();
            bool resultado = objCapaDato.ReestablecerClave(idcliente, CN_Recursos.ConvertirSha256(nuevaclave), out Mensaje);
            if (resultado)
            {
                string asunto = "Recuperacion Cuentas - Tienda Comercial";
                //string mensaje_correo = "<h3>Su cuenta fue reestablecida con exito</h3><br><p>Su clave para acceder a nuestro sistema es: !clave!</p>";
                string mensaje_correo = "<!DOCTYPE html><html lang='ES-SV' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'><head><meta charset='utf-8'><meta name='viewport' content='width=device-width'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='x-apple-disable-message-reformatting'><link href='https://fonts.googleapis.com/css?family=Lato:300,400,700' rel='stylesheet'></head><style>@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {u ~ div .email-container{min-width: 320px !important;}}@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {u ~ div .email-container{min-width: 375px !important;}}@media only screen and (min-device-width: 414px){u ~ div .email-container {min-width: 414px !important;}}</style><body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'><center style='width: 100%; background-color: #f1f1f1;'><div style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;</div><div style='max-width: 600px; margin: 0 auto;' class='email-container'><table style='mso-table-lspace: 0pt !important;mso-table-rspace: 0pt !important; border-spacing: 0 !important;border-collapse: collapse !important;table-layout: fixed !important;margin: 0 auto !important;' align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin: auto;'><tr><td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em; background: #ffffff;'><table style='mso-table-lspace: 0pt !important;mso-table-rspace: 0pt !important;border-spacing: 0 !important;border-collapse: collapse !important;table-layout: fixed !important;margin: 0 auto !important;' role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'><tr><td class='logo' style='text-align: center;'><h1><a href='#'><img style='width: 120px;' src='http://tiendacomercial.somee.com/Recursos/logo_transparent.png'></a></h1></td></tr></table></td></tr><tr><td valign='middle' class='hero bg_white' style='padding: 3em 0 2em 0; background: #ffffff;'><img src='http://tiendacomercial.somee.com/Recursos/message.gif' style='width: 300px; max-width: 200px; height: auto; margin: auto; display: block;'></td></tr><tr><td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0; background: #ffffff;'><table style='mso-table-lspace: 0pt !important;mso-table-rspace: 0pt !important;border-spacing: 0 !important;border-collapse: collapse !important;table-layout: fixed !important;margin: 0 auto !important;'><tr><td><div class='text' style='padding: 0 2.5em; text-align: center;'><h2 style='font-family: Tahoma, sans-serif;color: #000000;'>Recuperaci&oacute;n Cuentas</h2><br><h4 style='font-family: Tahoma, sans-serif;color: #000000;'>Estimado(a) cliente, usted ha solicitado una solicitud de recuperaci&oacute;n de cuentas. Hemos generado una nueva contrase&ntilde;a, por favor inicie sesi&oacute;n con su direcci&oacute;n de correo electr&oacute;nico e ingrese la misma.</h4><br><span style='padding: .8rem; font-size: 18px; font-family: Tahoma, sans-serif; color: #666666; line-height: 150%; border: 3px dashed #b2bec3; letter-spacing: 1rem;'>!clave!</span><p><a style='background: #c0392b; margin-top: 20px;padding: 10px 15px;display: inline-block; text-decoration: none; color: #fff; font-family: Tahoma, sans-serif;' class='btn'><strong>Nota: Si usted no ha iniciado el proceso de reestablecer contraseña, por favor realice el cambio de su contrase&ntilde;a lo antes posible. Le garantizamos la protecci&oacute;n de sus datos, siempre y cuando no comparta el contenido de este correo con otras personas.</strong></a></p></div></td></tr></table></td></tr></table><table style='mso-table-lspace: 0pt !important;mso-table-rspace: 0pt !important;border-spacing: 0 !important;border-collapse: collapse !important;table-layout: fixed !important;margin: 0 auto !important;' align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin: auto;'><tr><td valign='middle' class='bg_light footer email-section' style='background: #fafafa; border-top: 1px solid rgba(0,0,0,.05);color: rgba(0,0,0,.5); padding:2.5em;'><table style='mso-table-lspace: 0pt !important;mso-table-rspace: 0pt !important;border-spacing: 0 !important;border-collapse: collapse !important;table-layout: fixed !important;margin: 0 auto !important;'><tr><td valign='top' width='33.333%' style='padding-top: 20px;'><table style='mso-table-lspace: 0pt !important;mso-table-rspace: 0pt !important;border-spacing: 0 !important;border-collapse: collapse !important;table-layout: fixed !important;margin: 0 auto !important;' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'><tr><td style='text-align: left; padding-right: 10px;'><h3 style='font-family: Helvetica, sans-serif; color: #000000;' class='heading'> </h3><p style='font-family: Helvetica, sans-serif; color: #000000;'> </p><p style='font-family: Helvetica, sans-serif; color: #000000;'><strong>Dudas, Problemas Por favor comun&iacute;quese a: <span style='color: #44bd32;'>soporte@tiendacomercial.com</span></p></td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr><tr><td class='bg_light' style='text-align: center; background: #fafafa;'><p style='background: #000; color: #F20; font-family: Helvetica, sans-serif;'>Este correo ha sido generado autom&aacute;ticamente, por favor no responder</p></td></tr></table></div></center></body></html>";
                mensaje_correo = mensaje_correo.Replace("!clave!", nuevaclave);
                bool respuesta = CN_Recursos.EnviarCorreo(correo, asunto, mensaje_correo);
                if (respuesta)
                {
                    return true;
                }
                else
                {
                   Mensaje = "Lo sentimos, no fue posible enviar el correo";
                   return false;
                }
            }
            else
            {
                Mensaje = "Lo sentimos, no fue posible reestablecer la cuenta de este usuario";
                return false;
            }
       }
    }
}
