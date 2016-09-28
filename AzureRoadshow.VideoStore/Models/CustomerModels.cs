using AzureRoadshow.VideoStore.Models.EF;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models
{
    public class BillingModel
    {
        public BillingInformation billingInformation { get; set; }
        public Address billingAddress { get; set; }
        public Address shippingAddress { get; set; }
    }

    public class CheckoutModel
    {
        public decimal amount { get; set; }
        public bool isWishList { get; set; }
        public List<CartItem> shoppingCart { get; set; }
        public Customer customer { get; set; }
        public BillingModel billing { get; set; }
    }

    public class CustomerInfoModel
    {
        public CheckoutModel checkoutInfo { get; set; }
        public LocalPasswordModel password { get; set; }
        public List<TransactionHistory> transactions { get; set; }
    }
}