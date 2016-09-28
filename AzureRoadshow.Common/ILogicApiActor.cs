using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using System.Collections.Generic;
using EventHubActor.Interfaces;

namespace AzureRoadshow.Common
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface ILogicApiActor : IActor
    {
        Task SendVideoPurchaseText(VideoPurchase purchase);
        Task WriteRentalHistory(RentalHistory rentalHistory);
    }
}
