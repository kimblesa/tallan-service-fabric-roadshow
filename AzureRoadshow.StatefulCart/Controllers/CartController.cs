using AzureRoadshow.Common;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace StatefulCart.Controllers
{
    public class CartController : ApiController
    {
        private readonly IReliableStateManager stateManager;
        
        public CartController(IReliableStateManager stateManager) : base()
        {
            this.stateManager = stateManager;
        }

        /// <summary>
        /// add a new cart item for a given customer
        /// </summary>
        /// <param name="cartItem"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody]CartItem cartItem)
        {
            var cartDict = await stateManager
                .GetOrAddAsync<IReliableDictionary<int, IEnumerable<CartItem>>>("cartDictionary");

            using (ITransaction tx = stateManager.CreateTransaction())
            {
                ConditionalValue<IEnumerable<CartItem>> result = await cartDict
                    .TryGetValueAsync(tx, cartItem.CustomerId);
                var newList = new List<CartItem> { cartItem };

                if (result.HasValue)
                {
                    IEnumerable<CartItem> newCart = result.Value.Concat(newList);
                    await cartDict.SetAsync(tx, cartItem.CustomerId, newCart);
                }
                else
                {
                    await cartDict.AddAsync(tx, cartItem.CustomerId, newList);
                }

                await tx.CommitAsync();
            }

            return Ok();
        }

        /// <summary>
        /// get the entire cart for a given customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> Get(int customerId)
        {

            var cartDict = await stateManager
                .GetOrAddAsync<IReliableDictionary<int, IEnumerable<CartItem>>>("cartDictionary");

            using (ITransaction tx = stateManager.CreateTransaction())
            {
                ConditionalValue<IEnumerable<CartItem>> result = await cartDict
                    .TryGetValueAsync(tx, customerId);

                if (result.HasValue)
                {
                    return Ok(result.Value);
                }
            }

            return Ok(Enumerable.Empty<CartItem>());
        }

        /// <summary>
        /// update an existing cart item for a customer
        /// </summary>
        /// <remarks>
        /// does not add the item if it doesn't exist.
        /// </remarks>
        /// <param name="cartItem"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]CartItem cartItem)
        {
            var cartDict = await stateManager
                .GetOrAddAsync<IReliableDictionary<int, IEnumerable<CartItem>>>("cartDictionary");

            using (ITransaction tx = stateManager.CreateTransaction())
            {
                ConditionalValue<IEnumerable<CartItem>> result = await cartDict
                    .TryGetValueAsync(tx, cartItem.CustomerId);
                var newList = new List<CartItem> { cartItem };

                if (result.HasValue)
                {
                    var newCartList = result.Value.ToList();
                    var foundItem = newCartList.FindIndex(
                        delegate (CartItem it)
                        {
                            return it.InventoryId == cartItem.InventoryId;
                        });

                    // if the cart item exists, update it
                    if (foundItem != -1)
                    {
                        // if the new cart item has a zero quantity, remove it from the cart
                        if (cartItem.Quantity <= 0)
                        {
                            newCartList.RemoveAt(foundItem);
                        }
                        // otherwise update it
                        else
                        {
                            newCartList[foundItem] = cartItem;
                        }
                    }

                    await cartDict.SetAsync(tx, cartItem.CustomerId, newCartList);
                    await tx.CommitAsync();
                }
            }

            return Ok();
        }

        /// <summary>
        /// delete the cart associated with a given customer id.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int customerId)
        {
            var cartDict = await stateManager
                .GetOrAddAsync<IReliableDictionary<int, IEnumerable<CartItem>>>("cartDictionary");

            using (ITransaction tx = stateManager.CreateTransaction())
            {
                await cartDict.TryRemoveAsync(tx, customerId);
                await tx.CommitAsync();
            }

            return Ok();
        }
    }
}
