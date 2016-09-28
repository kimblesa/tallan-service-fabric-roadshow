using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class Inventory
    {
        public Inventory()
        {
            FormatToVideo = new HashSet<FormatToVideo>();
            VideoReview = new HashSet<VideoReview>();
        }

        public int InventoryId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        
        public virtual ICollection<FormatToVideo> FormatToVideo { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<VideoReview> VideoReview { get; set; }
    }
}
