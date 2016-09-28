using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class VideoReview
    {
        public int ReviewId { get; set; }
        public int InventoryId { get; set; }
        public string Review { get; set; }
        public short Rating { get; set; }
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Inventory Inventory { get; set; }
    }
}
