using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class BillingInformation
    {
        public int BillingId { get; set; }
        public string CreditCard { get; set; }
        public int AddressId { get; set; }
        public string Name { get; set; }

        public virtual Address Address { get; set; }
    }
}
