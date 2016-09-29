using AzureRoadshow.Common;
using EventHubActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace AzureRoadshow.PurchaseApi.Controllers
{
    [ServiceRequestActionFilter]
    [RoutePrefix("api/purchase")]
    public class PurchaseController : ApiController
    {
        IEventHubActor proxy;
        ILogicApiActor logicAppProxy;
        public PurchaseController()
        {
            proxy = ActorProxy.Create<IEventHubActor>(ActorId.CreateRandom());
            logicAppProxy = ActorProxy.Create<ILogicApiActor>(ActorId.CreateRandom());
        }

        // PUT api/purchase 
        [HttpPut]
        public void Put([FromBody]VideoPurchase value)
        {
            proxy.SubmitPurchase(value);
        }

        [ActionName("list")]
        [HttpPut]
        public void PutList([FromBody]IEnumerable<VideoPurchase> purchaseList)
        {
            proxy.SubmitPurchaseList(purchaseList);
        }

        [ActionName("random")]
        // POST api/purchase/random
        [HttpPost]
        public async void PostRandom()
        {
            Random rnd = new Random();

            await proxy.SubmitPurchase(VideoPurchaseFactory.CreateRandomPurchase(rnd));
        }

        [ActionName("randomList")]
        // POST api/purchase/randomList/5
        [HttpPost]
        public void PostRandomNumber([FromUri]int number)
        {
            Random rnd = new Random();
            proxy.SubmitPurchaseList(Enumerable.Range(1, number).Select(x => VideoPurchaseFactory.CreateRandomPurchase(rnd)));
        }


        // POST api/purchase/getrandom
        [HttpGet]
        public async Task GetRandom()
        {
            Random rnd = new Random();

            VideoPurchase purchase = VideoPurchaseFactory.CreateRandomPurchase(rnd);

            await logicAppProxy.SendVideoPurchaseText(purchase);
        }

        [ActionName("writerentalhistory")]
        // POST api/purchase/writerentalhistory
        [HttpPost]
        public async Task WritePurchaseHistory(RentalHistory rentalHistory)
        {
            await logicAppProxy.WriteRentalHistory(rentalHistory);
        }

    }
}
