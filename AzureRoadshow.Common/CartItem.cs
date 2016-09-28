using System.Runtime.Serialization;

namespace AzureRoadshow.Common
{
    [DataContract]
    public sealed class CartItem
    {
        [DataMember] public int CustomerId { get; set; }
        [DataMember]public int CartId { get; set; }
        [DataMember] public int InventoryId { get; set; }
        [DataMember] public int Quantity { get; set; }
        [DataMember] public bool IsWishList { get; set; }
    }
}
