namespace AzureRoadshow.VideoStore.Models
{
    // TODO this class is *supposed* to be in the common project,
    //  but it's a 4.5.2 framework and I have yet to modify the project
    //  so that it can be referenced from here.
    public class VideoPurchase
    {
        public int VideoID { get; set; }
        public int InventoryID { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
