using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class Customer
    {
        public Customer()
        {
            Address = new HashSet<Address>();
            VideoReview = new HashSet<VideoReview>();
        }

        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual ICollection<VideoReview> VideoReview { get; set; }
    }
}
