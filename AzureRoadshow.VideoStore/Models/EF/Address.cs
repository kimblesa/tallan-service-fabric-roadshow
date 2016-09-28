using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class Address
    {
        public Address()
        {
            BillingInformation = new HashSet<BillingInformation>();
        }

        public int AddressId { get; set; }
        public string AddressType { get; set; }
        public string Road { get; set; }
        public string Town { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int CustomerId { get; set; }

        public virtual ICollection<BillingInformation> BillingInformation { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
