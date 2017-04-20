# Tallan Service Fabric Roadshow

For the series of Azure/Visual Studio workshops Tallan is participating in, we have provided an implementation of an Azure Service Fabric solution, with interconnected examples for multiple reliable service types.

## Resources and Links
For an overview of Service Fabric, refer to Microsoft's documentation:
<br/><https://azure.microsoft.com/en-us/documentation/articles/service-fabric-overview/>

For another example of Service Fabric in action, refer to the Microsoft Samples Git repositories:
<br/><https://github.com/Azure-Samples/service-fabric-dotnet-getting-started>

Bear in mind that to open the following code in Visual Studio, you will need Visual Studio 2015, along with the Service Fabric SDK (The Azure SDK would be helpful as well):
<br/><https://azure.microsoft.com/en-us/documentation/articles/service-fabric-get-started/#install-the-runtime-sdk-and-tools>

Opening this project with Visual Studio 2017 has a few issues when attempting to open this project, so using 2015 is recommended at this time. 

## Overview

These projects rely heavily on new Service Fabric programming concepts and .NET Core. Azure Service Fabric implements a microservices model, primarily through the use of Stateful and Stateless Reliable Services, a variety of which are implemented in this solution.

This solution consists of the following projects:
* Service Fabric Project
* Two Reliable Actor Projects
* One ASP.NET Web API, based on the Stateless Reliable service model
* One ASP.NET Core Web Application, based on the Stateful Reliable model
* One common project containing shared interfaces and classes
* One Database project containing the schema depended on by the Web Application

### Service Fabric Project
The project containing publish information about the services, including their partition counts and a powershell script to deploy. Note that since the Video Store project has outside dependencies (on a database and external hosting for images), it is excluded to start with, but can be re-added. 

### Reliable Actor Projects
Reliable Actors are layers of abstraction Azure provides over Stateful Reliable services, which allow for messages to be reliably handled in a typed manner through a proxy as intermediary.
Two reliable actor services exist -- EventHubActor, and LogicApiActor. These are both called from the PurchaseApi project, in a manner similar to the following code snippet:

```C#
  public PurchaseController()
  {
      proxy = ActorProxy.Create<IEventHubActor>(ActorId.CreateRandom());
  }

  // PUT api/purchase 
  [HttpPut]
  public void Put([FromBody]VideoPurchase value)
  {
      proxy.SubmitPurchase(value);
  }
```

Both reliable services have configuration information in their respective PackageRoot/Config/Settings.xml. Look here to set an event hub connection string (for EventHubActor), or endpoints for any Logic Apps (for LogicApiActor).

### Purchase API
This is a Web API built based on the Stateless Reliable service model. The endpoint for this API is available at http://<clusteraddress>:8131/api/purchase, where the endpoints allow you to interact with the two Actor Services.

### Stateful Reliable Service
If you want to have a service listen over HTTP (using the easier RPC method is also possible, among others), you have to add an HTTP communication listener, as is demonstrated in this service. It uses OWIN as its hosting method. Also included here is simple Unity dependency injection to allow for the stateful object container to be injected into the web api controller.

### Web Application
A simple implementation of a Video Store built using ASP.NET Core with MVC and EF Core. This project has two external dependencies (besides dependencies on the service fabric applications) -- a SQL Server instance (I used Azure SQL), and optionally, a CDN for hosting images that the website uses.

A Database project, included in the solution, was used to do a schema compare against the database to populate tables and keys. From there, Entity Framework Core utilities were used to do a database-first model generation, as found here:
<br/><https://docs.efproject.net/en/latest/platforms/aspnetcore/existing-db.html>

This store uses the Stateful Reliable Cart to track purchases, and relies on the SQL database to perform individual user account authentication. The connection strings for this can be found and changed in the appsettings.json file, along with in the VideoStoreContext.cs class.

### Common project
Common models and interfaces are stored here. Still pending in the application is tweaks to allow for this class library to be referenced by the .NET Core web application; that work is still to be completed.
