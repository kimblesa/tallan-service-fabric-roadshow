using AzureRoadshow.VideoStore.Models.EF;

namespace AzureRoadshow.VideoStore.Models
{
    public class CartItem
    {
        public int CustomerId { get; set; }
        public int CartId { get; set; }
        public int InventoryId { get; set; }
        public int Quantity { get; set; }
        public bool IsWishList { get; set; }

        public Inventory Inventory { get; set; }
    }
}
