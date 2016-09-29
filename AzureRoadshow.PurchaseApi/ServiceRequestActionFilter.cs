using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AzureRoadshow.PurchaseApi
{
    internal class ServiceRequestActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            ServiceEventSource.Current.ServiceRequestStart(actionContext.ActionDescriptor.ActionName);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            ServiceEventSource.Current.ServiceRequestStop(actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
        }
    }
}
