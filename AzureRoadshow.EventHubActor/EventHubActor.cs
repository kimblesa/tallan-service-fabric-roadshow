using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using AzureRoadshow.Common;
using System.Fabric.Description;
using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Text;
using Newtonsoft.Json;
using Microsoft.ServiceFabric.Actors;

namespace AzureRoadshow.EventHubActor
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
    internal class EventHubActor : Actor, IEventHubActor
    {
        public EventHubActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {

        }

        private EventHubClient _eventHubClient;
        private const int BatchSize = 1000;

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
                // Make sure to update Settings.xml to refer to a valid Event Hub connection string
                _eventHubClient = EventHubClient.CreateFromConnectionString(GetEventHubConnectionString());
            }
            catch (Exception ex)
            {
                ActorEventSource.Current.ActorMessage(this, ex.Message);
            }

            return this.StateManager.TryAddStateAsync("count", 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task IEventHubActor.SubmitPurchaseList(IEnumerable<VideoPurchase> purchases)
        {
            try
            {
                var batchedMessages = purchases
                        .Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index / BatchSize)
                        .Select(x => x.Select(v => v.Value));

                List<Task> batchSendList = new List<Task>();

                foreach (var batch in batchedMessages)
                {
                    var batchedMessage = JsonConvert.SerializeObject(batch.ToArray());

                    batchSendList.Add(_eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(batchedMessage))));

                }

                Task.WaitAll(batchSendList.ToArray());
            }
            catch (Exception ex)
            {
                ActorEventSource.Current.ActorMessage(this, ex.Message);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task IEventHubActor.SubmitPurchase(VideoPurchase purchase)
        {
            try
            {
                var message = JsonConvert.SerializeObject(purchase);

                return _eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
            }
            catch (Exception ex)
            {
                ActorEventSource.Current.ActorMessage(this, ex.Message);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// pull connection string for Event Hub from the Settings.xml file in this project
        /// </summary>
        /// <returns></returns>
        private string GetEventHubConnectionString()
        {
            ConfigurationSettings settingsFile =
                ActorService.Context.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings;
            ConfigurationSection configSection = settingsFile.Sections["Config"];

            var connectionString = configSection.Parameters["EventHub"].Value;
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Did not find Service Bus connections string in appsettings (app.config)");
                return string.Empty;
            }
            try
            {
                var builder = new ServiceBusConnectionStringBuilder(connectionString);
                builder.TransportType = TransportType.Amqp;
                return builder.ToString();
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.Message);
                Console.ResetColor();
            }

            return null;
        }
    }
}
