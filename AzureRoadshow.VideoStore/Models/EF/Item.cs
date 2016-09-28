using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class Item
    {
        public int InventoryId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public virtual Inventory Inventory { get; set; }
    }
}
