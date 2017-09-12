using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using AzureRoadshow.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using EventHubActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System.Fabric.Description;

namespace LogicApiActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class LogicApiActor : Actor, ILogicApiActor
    {
        private string _rentalLogicAppEndpoint;
        private string _purchaseLogicAppEndpoint;

        public LogicApiActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
            
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see http://aka.ms/servicefabricactorsstateserialization

            try
            {
                // Make sure to update Settings.xml to refer to valid endpoints
                _rentalLogicAppEndpoint = GetConfigValue("RentalLogicAppEndpoint");
                _purchaseLogicAppEndpoint = GetConfigValue("PurchaseLogicAppEndpoint");
            }
            catch (Exception ex)
            {
                ActorEventSource.Current.ActorMessage(this, ex.Message);
            }

            return StateManager.TryAddStateAsync("count", 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rentalHistory"></param>
        /// <returns></returns>
        async Task ILogicApiActor.WriteRentalHistory(RentalHistory rentalHistory)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_rentalLogicAppEndpoint);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                    var content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        customerId = rentalHistory.CustomerId,
                        inventoryId = rentalHistory.InventoryId,
                        videoId = rentalHistory.VideoId,
                        startDate = rentalHistory.StartDate.ToString("MMMM dd, yyyy"),
                        endDate = rentalHistory.EndDate.ToString("MMMM dd, yyyy"),
                        transactionId = rentalHistory.TransactionId
                    }), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(string.Empty, content);
                }

            }

            catch (Exception ex)
            {
                ActorEventSource.Current.ActorMessage(this, ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        async Task ILogicApiActor.SendVideoPurchaseText(VideoPurchase purchase)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_purchaseLogicAppEndpoint);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                    string copyString = purchase.Quantity == 1 ? "copy" : "copies";
                    var content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        quantity = purchase.Quantity,
                        price = purchase.Price,
                        title = purchase.Title,
                        phoneNumber = "##########",
                        copyString = copyString
                    }), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(string.Empty, content);
                }

            }
            catch (Exception ex)
            {
                ActorEventSource.Current.ActorMessage(this, ex.Message);
            }
        }

        private string GetConfigValue(string valueName)
        {
            ConfigurationSettings settingsFile =
                ActorService.Context.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings;
            ConfigurationSection configSection = settingsFile.Sections["Config"];

            var result = configSection.Parameters[valueName].Value;

            return result;
        }
    }
}
