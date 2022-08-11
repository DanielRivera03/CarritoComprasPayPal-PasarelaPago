using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Paypal
{
    public class Response_Checkout
    {
        public string id { get; set; }
        public string status { get; set; }
        public List<CC_Link> links { get; set; }

    }

    public class CC_Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }
}
