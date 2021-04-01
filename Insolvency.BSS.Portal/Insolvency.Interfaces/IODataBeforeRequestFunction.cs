using System.Net.Http;
using System.Threading.Tasks;

namespace Insolvency.Interfaces
{
    public interface IODataBeforeRequestFunction
    {
        public Task BeforeRequestAsync(HttpRequestMessage message);
    }
}
