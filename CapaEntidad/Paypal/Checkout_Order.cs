using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Paypal
{
    public class Checkout_Order
    {
        public string intent { get; set; }
        public List<PurchaseUnit> purchase_units { get; set; }
        public ApplicationContext application_context { get; set; }
    }


    public class ItemTotal
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Breakdown
    {
        public ItemTotal item_total { get; set; }
    }

    public class Amount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
        public Breakdown breakdown { get; set; }
    }

    public class UnitAmount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Item
    {
        public string name { get; set; }
        public string quantity { get; set; }
        public UnitAmount unit_amount { get; set; }
    }

    public class PurchaseUnit
    {
        public Amount amount { get; set; }
        public string description { get; set; }
        public List<Item> items { get; set; }
    }


    public class ApplicationContext
    {
        public string brand_name { get; set; }
        public string landing_page { get; set; }
        public string user_action { get; set; }
        public string return_url { get; set; }
        public string cancel_url { get; set; }
    }
}
