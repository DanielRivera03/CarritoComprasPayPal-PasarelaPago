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
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using CapaEntidad.Paypal;

namespace CapaNegocio
{
    public class CN_Paypal
    {
        private static string urlpaypal = ConfigurationManager.AppSettings["UrlPaypal"];
        private static string clientId = ConfigurationManager.AppSettings["ClientId"];
        private static string secret = ConfigurationManager.AppSettings["Secret"];

        // CREAR SOLICITUD DE PAGO -> SERVICIO PROVISTO POR PAYPAL
        public async Task<Response_Paypal<Response_Checkout>> CrearSolicitud(Checkout_Order orden)
        {
            Response_Paypal <Response_Checkout> response_paypal = new Response_Paypal<Response_Checkout>();
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlpaypal);
                var authToken = Encoding.ASCII.GetBytes($"{clientId}:{secret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",Convert.ToBase64String(authToken));
                var json = JsonConvert.SerializeObject(orden);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("/v2/checkout/orders", data);
                response_paypal.Status = response.IsSuccessStatusCode;
                if (response.IsSuccessStatusCode)
                {
                    string jsonRespuesta = response.Content.ReadAsStringAsync().Result;
                    Response_Checkout checkout = JsonConvert.DeserializeObject<Response_Checkout>(jsonRespuesta);
                    response_paypal.Response = checkout;
                }
                return response_paypal;
            }
        }

        // APROBAR TRANSACCION SI EXISTEN FONDOS
        public async Task<Response_Paypal<Response_Capture>> AprobarPago(string token)
        {
            Response_Paypal<Response_Capture> response_paypal = new Response_Paypal<Response_Capture>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlpaypal);
                var authToken = Encoding.ASCII.GetBytes($"{clientId}:{secret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
                var data = new StringContent("{}", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"/v2/checkout/orders/{token}/capture", data);
                response_paypal.Status = response.IsSuccessStatusCode;
                if (response.IsSuccessStatusCode)
                {
                    string jsonRespuesta = response.Content.ReadAsStringAsync().Result;
                    Response_Capture capture = JsonConvert.DeserializeObject<Response_Capture>(jsonRespuesta);
                    response_paypal.Response = capture;
                }
                return response_paypal;
            }
        }


    }
}
