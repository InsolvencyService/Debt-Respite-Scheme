using System.Text.Json;

namespace Insolvency.IntegrationAPI.ErrorHandling
{
    public class Error
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public CustomError GetCustomError()
        {
            return JsonSerializer.Deserialize<CustomError>(Message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
