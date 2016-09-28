using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace AzureRoadshow.Common
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IEventHubActor : IActor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task SubmitPurchase(VideoPurchase purchase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchases"></param>
        /// <returns></returns>
        Task SubmitPurchaseList(IEnumerable<VideoPurchase> purchases);
    }
}
