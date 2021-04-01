namespace Insolvency.IntegrationAPI.ErrorHandling
{
    public class DynamicsError
    {
        public Error Error { get; set; }

        public CustomError GetCustomError()
        {
            return Error.GetCustomError();
        }
    }
}
