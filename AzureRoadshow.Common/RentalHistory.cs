using System;

namespace EventHubActor.Interfaces
{
    public class RentalHistory
    {
        public int CustomerId { get; set; }
        public int InventoryId { get; set; }
        public int VideoId { get; set; }
        public int TransactionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
