using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class TransactionHistory
    {
        public int TransactionId { get; set; }
        public int CustomerId { get; set; }
        public int InventoryId { get; set; }
        public DateTime? Date { get; set; }

        public virtual RentalHistory RentalHistory { get; set; }
    }
}
