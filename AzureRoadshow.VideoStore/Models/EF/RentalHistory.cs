using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class RentalHistory
    {
        public int TransactionId { get; set; }
        public int CustomerId { get; set; }
        public int InventoryId { get; set; }
        public string VideoId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual TransactionHistory Transaction { get; set; }
    }
}
