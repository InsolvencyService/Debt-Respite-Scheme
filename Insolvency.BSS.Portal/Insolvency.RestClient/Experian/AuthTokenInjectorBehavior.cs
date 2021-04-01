using System.Collections.Generic;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Insolvency.RestClient.Experian
{
    public class AuthTokenInjectorBehavior : IEndpointBehavior
    {
        public IEnumerable<IClientMessageInspector> MessageInspectors { get; }

        public AuthTokenInjectorBehavior(params IClientMessageInspector[] messageInspectors)
        {
            this.MessageInspectors = messageInspectors;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            foreach (var inspector in MessageInspectors)
            {
                clientRuntime.ClientMessageInspectors.Add(inspector);
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        { }

        public void Validate(ServiceEndpoint endpoint)
        { }
    }
}
