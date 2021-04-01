using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Insolvency.RestClient.Experian
{
    public class TokenInjectionInspector : IClientMessageInspector
    {
        public const string USER_AGENT_HTTP_HEADER = "Auth-Token";
        public string Token { get; }

        public TokenInjectionInspector(string token)
        {
            this.Token = token;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        { }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequestMessage;
            object httpRequestMessageObject;
            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
            {
                httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
                if (string.IsNullOrEmpty(httpRequestMessage.Headers[USER_AGENT_HTTP_HEADER]))
                {
                    httpRequestMessage.Headers[USER_AGENT_HTTP_HEADER] = this.Token;
                }
                return null;
            }

            httpRequestMessage = new HttpRequestMessageProperty();
            httpRequestMessage.Headers.Add(USER_AGENT_HTTP_HEADER, this.Token);
            request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
            return null;
        }
    }
}
