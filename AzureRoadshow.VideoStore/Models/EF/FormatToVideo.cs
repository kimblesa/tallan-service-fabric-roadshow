using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class FormatToVideo
    {
        public int RelationId { get; set; }
        public int FormatId { get; set; }
        public int VideoId { get; set; }
        public int InventoryId { get; set; }

        public virtual VideoFormat Format { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual Video Video { get; set; }
    }
}
