using System;
using System.Linq;

namespace AzureRoadshow.Common
{
    public class VideoPurchaseFactory
    {
        private static readonly int VideoCount = 2;
        private static readonly int InventoryCount = 100;
        private static readonly int MaxQuantity = 1;
        private static readonly double PricePerItem = 9.99f;

        private static readonly string[] Titles =
        {
            "The Silence of the Lambs",
            "Bill and Ted's Excellent Adventure",
            "Ex Machina",
            "Young Frankenstein",
            "A Night at the Roxbury",
            "Nosferatu",
            "The Man Who Would Be King",
            "North By Northwest",
            "The Maltese Falcon",
            "The Good, the Bad, and the Ugly",
        };

        public static VideoPurchase CreateRandomPurchase(Random rnd)
        {
            try
            {
                int quantity = rnd.Next(1, MaxQuantity);

                var result = new VideoPurchase
                {
                    VideoID = rnd.Next(1, VideoCount),
                    InventoryID = rnd.Next(1, InventoryCount),
                    Title = Titles.ElementAt(rnd.Next(Titles.Count())),
                    Quantity = quantity,
                    Price = Math.Round(PricePerItem * quantity, 2, MidpointRounding.AwayFromZero)
                };

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
