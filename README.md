# Tallan Service Fabric Roadshow

For the series of Azure/Visual Studio workshops Tallan is participating in, we have provided an implementation of an Azure Service Fabric solution, with simple examples for multiple reliable service types.

The next Tallan-hosted workshop will be in Massachusetts in October:
<br/><https://www.eventbrite.com/e/microsoft-azure-visual-studio-workshop-boston-tickets-27183061268>

## Resources and Links
For an overview of Service Fabric, refer to Microsoft's documentation:
<br/><https://azure.microsoft.com/en-us/documentation/articles/service-fabric-overview/>

For another example of Service Fabric in action, refer to the Microsoft Samples Git repositories:
<br/><https://github.com/Azure-Samples/service-fabric-dotnet-getting-started>

Bear in mind that to open the following code in Visual Studio, you will need Visual Studio 2015, along with the Service Fabric SDK (The Azure SDK would be helpful as well):
<br/><https://azure.microsoft.com/en-us/documentation/articles/service-fabric-get-started/#install-the-runtime-sdk-and-tools>

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

### Reliable Actor Projects

### Purchase API

### Web Application

### Common project

### Database project
