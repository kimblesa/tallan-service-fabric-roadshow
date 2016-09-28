using Microsoft.Practices.Unity;
using Microsoft.ServiceFabric.Data;
using StatefulCart.Controllers;
using System.Web.Http;
using Unity.WebApi;

namespace StatefulCart
{
    public static class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration config, IReliableStateManager stateManager)
        {
			var container = new UnityContainer();

            container.RegisterType<CartController>(
                new TransientLifetimeManager(),
                new InjectionConstructor(stateManager));
            
            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}