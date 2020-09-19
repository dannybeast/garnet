using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    public class IcbcodeCartItem
    {
        public long item_id { get; set; }
        public string item_name { get; set; }
        public string item_description { get; set; }
        public string item_url { get; set; }
        public decimal item_price { get; set; }
        public string item_unit { get; set; }
        public int item_count { get; set; }
        public int item_discount { get; set; }
    }

    public class IcbcodeCart
    {
        private const string cart_session_key = "shopping_cart_info";

        private const string cart_promocode_session_key = "shopping_cart_promocode";

        private const string cart_delivery_method_session_key = "shopping_cart_delivery_method";

        public List<IcbcodeCartItem> Items { get; private set; }

        public string Promocode { get; private set; }

        public string DeliveryMethod { get; private set; }

        public IcbcodeCart(HttpContextBase context)
        {
            Items = (List<IcbcodeCartItem>)context.Session[cart_session_key];

            if (Items == null)
            {
                Items = new List<IcbcodeCartItem>();

                context.Session[cart_session_key] = Items;
            }

            Promocode = (string)context.Session[cart_promocode_session_key];

            DeliveryMethod = (string)context.Session[cart_delivery_method_session_key];
        }

        public void SetPromocode(string promocode)
        {
            Promocode = promocode;
        }

        public void SetDeliveryMethod(string deliveryMethod)
        {
            DeliveryMethod = deliveryMethod;
        }

        public void AddToCart(long item_id, string item_name, string item_description, string item_url, decimal item_price, string item_unit, int item_discount, int product_count = 1)
        {
            IcbcodeCartItem cart_item = Items.Find(delegate (IcbcodeCartItem item) { return item.item_id == item_id; });

            if (cart_item == null)
            {
                Items.Add(new IcbcodeCartItem() { item_id = item_id, item_name = item_name, item_description = item_description, item_url = item_url, item_price = item_price, item_unit = item_unit, item_count = product_count, item_discount = item_discount });
            }
            else
            {
                cart_item.item_count = cart_item.item_count + 1;
            }
        }

        public void RemoveFromCart(long item_id)
        {
            RemoveFromCart(item_id, 1);
        }

        public void RemoveFromCart(long item_id, int item_count)
        {
            IcbcodeCartItem cart_item = Items.Find(delegate (IcbcodeCartItem item) { return item.item_id == item_id; });

            if (cart_item != null)
            {
                if (cart_item.item_count > 1 && (cart_item.item_count - item_count) > 0)
                {
                    cart_item.item_count = cart_item.item_count - 1;
                }
                else
                {
                    Items.Remove(cart_item);
                }
            }
        }

        public void EmptyCart()
        {
            Items.Clear();

            Promocode = null;

            DeliveryMethod = null;
        }

        public int GetCount()
        {
            int count = 0;

            foreach (IcbcodeCartItem cart_item in Items)
            {
                count += cart_item.item_count;
            }

            return count;
        }

        public decimal GetTotal()
        {
            decimal total = decimal.Zero;

            foreach (IcbcodeCartItem cart_item in Items)
            {
                total += (cart_item.item_price - (cart_item.item_price / 100 * cart_item.item_discount)) * cart_item.item_count;
            }

            return total;
        }
    }
}