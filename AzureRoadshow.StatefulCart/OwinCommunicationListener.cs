// Thanks to Microsoft for providing this code, to which I made minor tweaks --
// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Owin;

namespace StatefulCart
{
    internal class OwinCommunicationListener : ICommunicationListener
    {
        private readonly ServiceEventSource eventSource;
        private readonly Action<IAppBuilder> startup;
        private readonly ServiceContext serviceContext;
        private readonly string endpointName;
        private readonly string appRoot;

        private IDisposable webApp;
        private string publishAddress;
        private string listeningAddress;

        public OwinCommunicationListener(Action<IAppBuilder> startup, ServiceContext serviceContext, ServiceEventSource eventSource, string endpointName)
            : this(startup, serviceContext, eventSource, endpointName, null)
        {
        }

        public OwinCommunicationListener(Action<IAppBuilder> startup, ServiceContext serviceContext, ServiceEventSource eventSource, string endpointName, string appRoot)
        {
            if (startup == null)
            {
                throw new ArgumentNullException(nameof(startup));
            }

            if (serviceContext == null)
            {
                throw new ArgumentNullException(nameof(serviceContext));
            }

            if (endpointName == null)
            {
                throw new ArgumentNullException(nameof(endpointName));
            }

            if (eventSource == null)
            {
                throw new ArgumentNullException(nameof(eventSource));
            }

            this.startup = startup;
            this.serviceContext = serviceContext;
            this.endpointName = endpointName;
            this.eventSource = eventSource;
            this.appRoot = appRoot;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var serviceEndpoint = serviceContext.CodePackageActivationContext.GetEndpoint(endpointName);
            var protocol = serviceEndpoint.Protocol;
            int port = serviceEndpoint.Port;

            if (serviceContext is StatefulServiceContext)
            {
                StatefulServiceContext statefulServiceContext = serviceContext as StatefulServiceContext;

                listeningAddress = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}://+:{1}/{2}",
                    protocol,
                    port,
                    string.IsNullOrWhiteSpace(appRoot)
                        ? string.Empty
                        : appRoot.TrimEnd('/') + '/');
            }
            else if (serviceContext is StatelessServiceContext)
            {
                listeningAddress = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}://+:{1}/{2}",
                    protocol,
                    port,
                    string.IsNullOrWhiteSpace(appRoot)
                        ? string.Empty
                        : appRoot.TrimEnd('/') + '/');
            }
            else
            {
                throw new InvalidOperationException();
            }

            publishAddress = listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            try
            {
                eventSource.Message("Starting web server on " + listeningAddress);

                webApp = WebApp.Start(listeningAddress, appBuilder => startup.Invoke(appBuilder));

                eventSource.Message("Listening on " + publishAddress);

                return Task.FromResult(publishAddress);
            }
            catch (Exception ex)
            {
                eventSource.Message("Web server failed to open endpoint {0}. {1}", endpointName, ex.ToString());

                StopWebServer();

                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            eventSource.Message("Closing web server on endpoint {0}", endpointName);

            StopWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            eventSource.Message("Aborting web server on endpoint {0}", endpointName);

            StopWebServer();
        }

        private void StopWebServer()
        {
            if (webApp != null)
            {
                try
                {
                    webApp.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // no-op
                }
            }
        }
    }
}
