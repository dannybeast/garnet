using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace IcbcodeCMS.Areas.CMS.Utilities
{
    public class EOFD
    {
        private const string USERNAME = "231299409080";
        private const string PASSWORD = "F5E4DB0C-3AC1-48FF-80E5-DF8C6D950B8A";

        public void SendCheck(string email, List<ProductItem> items)
        {
            RequestRoot request = new RequestRoot();
            request.bill = new Bill();
            request.bill.buyerAddress = email;
            request.bill.ecashTotalSum = items.Sum(x => x.price * x.quantity);
            request.bill.operationType = 1;
            request.bill.userPass = "29";
            request.bill.items = items;

            string response = CreateRequest(JsonConvert.SerializeObject(request));

            if(response != null)
            {
                var result = JsonConvert.DeserializeObject<ResponseRoot>(response);
            }
        }

        private string CreateRequest(string json)
        {
            string result;

            using (WebClient client = new WebClient())
            {

                client.Credentials = new NetworkCredential(USERNAME, PASSWORD);

                result = Encoding.UTF8.GetString(client.UploadData("https://micropay.e-ofd.ru:2005/01801810000480/Transaction", "POST", Encoding.UTF8.GetBytes(json)));
            }

            return result;
        }
    }

    public class Error
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class Doc
    {
        public int fiscalDocumentNumber { get; set; }
        public long fiscalSignShort { get; set; }
        public string docType { get; set; }
    }

    public class ResponseRoot
    {
        public string kktNumber { get; set; }
        public Error Error { get; set; }
        public Doc doc { get; set; }
    }

    #region request object
    public class ProductItem
    {
        public string name { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public int VATrate { get; set; }
    }

    public class Bill
    {
        public string userPass { get; set; }
        public int operationType { get; set; }
        public decimal ecashTotalSum { get; set; }
        public string buyerAddress { get; set; }
        public List<ProductItem> items { get; set; }
    }

    public class RequestRoot
    {
        public Bill bill { get; set; }
    }
    #endregion
}