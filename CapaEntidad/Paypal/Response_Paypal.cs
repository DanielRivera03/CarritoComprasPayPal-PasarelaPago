using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Paypal
{
    public class Response_Paypal<T>
    {
        public bool Status { get; set; }
        public T Response { get; set; }
    }
}
