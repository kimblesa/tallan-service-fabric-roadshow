using AzureRoadshow.VideoStore.Models;
using AzureRoadshow.VideoStore.Models.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using AzureRoadshow.VideoStore.Services;
using Microsoft.ServiceFabric.Services.Communication.Client;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Client;
using System;
using System.Fabric.Query;
using System.Text;

namespace AzureRoadshow.VideoStore.Controllers
{
    [Authorize]
    public class PurchaseController : Controller
    {
        VideoStoreContext _db;
        UserManager<ApplicationUser> _userManager;

        private readonly FabricClient fabricClient;
        private readonly HttpCommunicationClientFactory commFactory;
        private readonly Uri cartServiceUri;
        private readonly Uri purchaseServiceApiUri;

        public PurchaseController(UserManager<ApplicationUser> userManager, VideoStoreContext context)
        {
            cartServiceUri = new Uri(FabricRuntime.GetActivationContext()
                .ApplicationName + "/StatefulCart");
            purchaseServiceApiUri = new Uri(FabricRuntime.GetActivationContext()
                .ApplicationName + "/PurchaseApi");

            _db = context;
            _userManager = userManager;

            fabricClient = new FabricClient();
            commFactory = new HttpCommunicationClientFactory(
                new ServicePartitionResolver(() => fabricClient));
        }

        /// <summary>
        /// return all cart items associated with the current user
        /// render cart items depending on whether they are in the wishlist or not
        /// </summary>
        /// <param name="isWishlist">Flag whether the cart to be returned is the wishlist.</param>
        /// <returns></returns>
        public ActionResult Cart(bool isWishlist)
        {
            var userId = _userManager.GetUserId(User);

            CheckoutModel cart = new CheckoutModel();
            cart.billing = new BillingModel();
            cart.isWishList = isWishlist;

            try
            {
                cart.customer = _db.Customer
                        .First(m => m.UserId == userId);

                cart.shoppingCart = GetCart(cart.customer.CustomerId)
                    .Where(x => x.IsWishList == isWishlist)
                    .ToList();

                // get total cost of items * quantities
                cart.amount = 0;
                foreach (var item in cart.shoppingCart)
                {
                    cart.amount += (item.Inventory.Price * item.Quantity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View("~/Views/Cart/Cart.cshtml", cart);
        }

        /// <summary>
        /// Populate and return the custom checkout model, containing billing and shipping information, along with the shopping cart.
        /// </summary>
        /// <param name="customerId">the customer to get the cart information for</param>
        /// <returns></returns>
        public ActionResult Checkout(int customerId)
        {
            CheckoutModel cart = new CheckoutModel();
            // initialize the sub-model as well
            cart.billing = new BillingModel();

            try
            {
                // query entity model for customer and relevant addresses/billing information
                Customer customer = _db.Customer
                    .Include(cust => cust.Address)
                        .ThenInclude(add => add.BillingInformation)
                    .First(m => m.CustomerId == customerId);

                cart.customer = customer;

                cart.shoppingCart = GetCart(cart.customer.CustomerId)
                    .Where(x => !x.IsWishList)
                    .ToList();

                // sum the total cost of the cart items
                cart.amount = 0;
                foreach (var item in cart.shoppingCart)
                {
                    cart.amount += (item.Inventory.Price * item.Quantity);
                }

                cart.billing.billingAddress = customer.Address.FirstOrDefault(m => m.AddressType == "Billing");
                cart.billing.shippingAddress = customer.Address.FirstOrDefault(m => m.AddressType == "Shipping");
                if (cart.billing.billingAddress != null)
                {
                    cart.billing.billingInformation = cart.billing.billingAddress.BillingInformation.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return View("~/Views/Cart/Checkout.cshtml", cart);
        }

        /// <summary>
        /// make the purchase, and update the transaction history
        /// TODO update rental history
        /// TODO actually implement payment type of some kind
        /// </summary>
        /// <param name="customerId">the id of the customer making the payment</param>
        /// <param name="paymentType">how the payment will be made. Currently just a stub</param>
        /// <returns></returns>
        public ActionResult MakePurchase(int customerId, string paymentType)
        {
            bool success = false;

            try
            {
                // TODO SAK replace this with call to purchase API
                Customer customer = _db.Customer
                    .FirstOrDefault(m => m.CustomerId == customerId);

                var purchases = GetCart(customer.CustomerId)
                    .Where(x => !x.IsWishList);

                var purchaseSubmission = purchases.Select(x => new VideoPurchase
                {
                    VideoID = x.InventoryId,
                    InventoryID = x.InventoryId,
                    Title = x.Inventory.Description,
                    Quantity = x.Quantity,
                    Price = (double)(x.Inventory.Price * x.Quantity)
                });

                var purchaseApiPartitionClient = new ServicePartitionClient<HttpCommunicationClient>(
                        commFactory,
                        purchaseServiceApiUri,
                        GetPartitionKey(customerId, purchaseServiceApiUri));

                purchaseApiPartitionClient.InvokeWithRetry(client =>
                {
                    var content = new StringContent(JsonConvert.SerializeObject(purchaseSubmission), Encoding.UTF8, "application/json");

                    var response = client.HttpClient.PutAsync(
                        new Uri(client.Url,
                                "api/purchase/list"),
                        content).Result;

                    success = response.IsSuccessStatusCode;
                });

                if (success && DeleteCart(customer.CustomerId))
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Add an item to a cart or wishlist
        /// </summary>
        /// <param name="inventoryId">the id of the inventory item to add</param>
        /// <param name="isWishlist">whether the item should be added to the cart or wishlist</param>
        /// <returns></returns>
        public ActionResult AddToCart(int inventoryId, bool isWishlist)
        {
            bool success = false;
            string cartType = (isWishlist) ? "Wishlist" : "Cart";
            CartItem newCartItem;

            var userId = _userManager.GetUserId(User);
            var rand = new Random();

            newCartItem = new CartItem
            {
                InventoryId = inventoryId,
                IsWishList = isWishlist,
                Quantity = 1,
                CustomerId = _db.Customer.First(m => m.UserId == userId).CustomerId,
                CartId = rand.Next(1, 1000)
            };

            try
            {
                var partitionClient = new ServicePartitionClient<HttpCommunicationClient>(
                        commFactory,
                        cartServiceUri,
                        GetPartitionKey(newCartItem.CustomerId, cartServiceUri));

                partitionClient.InvokeWithRetry(client =>
                {
                    var content = new StringContent(JsonConvert.SerializeObject(newCartItem), Encoding.UTF8, "application/json");

                    var response = client.HttpClient.PutAsync(
                        new Uri(client.Url,
                                string.Format("api/cart",
                                    newCartItem.CustomerId)),
                        content).Result;

                    success = response.IsSuccessStatusCode;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            // TODO SAK redirect to error if not successful
            return RedirectToAction("Cart", "Purchase", new { isWishlist = isWishlist });
        }

        /// <summary>
        /// remove an inventory id from the list of carts.
        /// There can (ideally) be only one inventory ID in *all* carts, regular or wishlist, so this removes only a single one.
        /// </summary>
        /// <param name="inventoryId">the id of the inventory item to delete</param>
        /// <returns></returns>
        public ActionResult DeleteFromCart(int inventoryId, bool isWishlist)
        {
            try
            {
                var customerId = _db.Customer.First(m => m.UserId == _userManager.GetUserId(User)).CustomerId;
                var cartItem = GetCart(customerId).FirstOrDefault(x => x.InventoryId == inventoryId);

                cartItem.Quantity = 0;

                bool success = UpdateCartItem(cartItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Cart", "Purchase", new { isWishlist = isWishlist });
        }

        /// <summary>
        /// change quantity.
        /// </summary>
        /// <param name="modifiedCart">Takes information from partial form submission in cart</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeQuantity(CartItem modifiedCart)
        {
            bool success = UpdateCartItem(modifiedCart);

            return RedirectToAction("Cart", "Purchase", new { isWishlist = modifiedCart.IsWishList });
        }

        /// <summary>
        /// switch item between cart and wishlist
        /// </summary>
        /// <param name="inventoryId">The inventory item to move</param>
        /// <param name="isWishlist">Which list is being modified</param>
        /// <returns></returns>
        public ActionResult SwitchCart(int inventoryId, bool isWishlist)
        {
            try
            {
                var customerId = _db.Customer.First(m => m.UserId == _userManager.GetUserId(User)).CustomerId;
                var cartItem = GetCart(customerId).FirstOrDefault(x => x.InventoryId == inventoryId);

                cartItem.IsWishList = isWishlist;

                bool success = UpdateCartItem(cartItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Cart", "Purchase", new { isWishlist = !(isWishlist) });
        }

        private bool UpdateCartItem(CartItem newCartItem)
        {
            try
            {
                var partitionClient = new ServicePartitionClient<HttpCommunicationClient>(
                        commFactory,
                        cartServiceUri,
                        GetPartitionKey(newCartItem.CustomerId, cartServiceUri));

                partitionClient.InvokeWithRetry(client =>
                {
                    var content = new StringContent(JsonConvert.SerializeObject(newCartItem), Encoding.UTF8, "application/json");

                    var response = client.HttpClient.PostAsync(
                        new Uri(client.Url,
                                string.Format("api/cart",
                                    newCartItem.CustomerId)),
                        content).Result;

                    return response.IsSuccessStatusCode;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        private bool DeleteCart(int customerId)
        {
            bool success = false;

            try
            {
                var partitionClient = new ServicePartitionClient<HttpCommunicationClient>(
                        commFactory,
                        cartServiceUri,
                        GetPartitionKey(customerId, cartServiceUri));

                partitionClient.InvokeWithRetry(client =>
                {
                    var response = client.HttpClient.DeleteAsync(
                        new Uri(client.Url,
                                string.Format("api/cart?customerId={0}",
                                    customerId))).Result;

                    success = response.IsSuccessStatusCode;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        private IEnumerable<CartItem> GetCart(int customerId)
        {
            var result = Enumerable.Empty<CartItem>();

            try
            {
                var partitionClient = new ServicePartitionClient<HttpCommunicationClient>(
                        commFactory,
                        cartServiceUri,
                        GetPartitionKey(customerId, cartServiceUri));

                partitionClient.InvokeWithRetry(client =>
                {
                    var response = client.HttpClient.GetAsync(
                        new Uri(client.Url,
                                string.Format("api/cart?customerId={0}",
                                    customerId))).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = response.Content.ReadAsStringAsync().Result;
                        result =
                            JsonConvert.DeserializeObject<IEnumerable<CartItem>>(responseString)
                            ?? Enumerable.Empty<CartItem>();
                    }
                });

                // populate inventory from DB
                var inventory = _db.Inventory
                    .Where(x => result.Select(y => y.InventoryId)
                            .Contains(x.InventoryId))
                    .ToList();

                foreach (var item in result)
                {
                    item.Inventory = inventory.Single(x => x.InventoryId == item.InventoryId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// get partition key for the given Service Fabric service uri.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="serviceUri"></param>
        /// <returns></returns>
        private ServicePartitionKey GetPartitionKey(int customerId, Uri serviceUri)
        {
            ServicePartitionList partitionList = fabricClient.QueryManager.GetPartitionListAsync(serviceUri).Result;

            // simple approach is used here -- just get the first available partition
            // for more information on partitioning, refer to the Microsoft documentation:
            // https://azure.microsoft.com/en-us/documentation/articles/service-fabric-concepts-partitioning/
            var partitionInformation = partitionList.First().PartitionInformation;
            
            if (partitionInformation.Kind == ServicePartitionKind.Int64Range)
            {
                var intRangePartitionInfo = (Int64RangePartitionInformation)partitionInformation;
                return new ServicePartitionKey(intRangePartitionInfo.LowKey);
            }
            // a singleton has only one partition, use the default constructor
            else if (partitionInformation.Kind == ServicePartitionKind.Singleton)
            {
                return new ServicePartitionKey();
            }
            else if (partitionInformation.Kind == ServicePartitionKind.Named)
            {
                var stringNamedPartitionInfo = (NamedPartitionInformation)partitionInformation;
                return new ServicePartitionKey(stringNamedPartitionInfo.Name);
            }
            else
            {
                throw new Exception(string.Format("Unexpected partition type: {0}", partitionInformation.Kind));
            }
            
        }
    }
}
