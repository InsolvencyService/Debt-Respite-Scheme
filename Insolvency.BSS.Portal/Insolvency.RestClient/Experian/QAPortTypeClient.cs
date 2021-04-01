using System.ServiceModel.Description;


namespace Insolvency.RestClient.Experian
{
    public partial class QAPortTypeClient : IQAPortTypeClient
    {
        public QAPortTypeClient(IEndpointBehavior endpointBehavior)
            : this()
        {
            this.ChannelFactory.Endpoint.EndpointBehaviors.Add(endpointBehavior);
        }
    }
}
