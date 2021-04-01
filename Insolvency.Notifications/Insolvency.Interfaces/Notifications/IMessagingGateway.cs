using System.Threading.Tasks;

namespace Insolvency.Interfaces.Notifications
{
    public interface IMessagingGateway
    {
        Task SendMessageAsync(byte[] message);
    }
}
