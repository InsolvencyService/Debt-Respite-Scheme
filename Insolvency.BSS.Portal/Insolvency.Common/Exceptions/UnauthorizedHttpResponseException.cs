namespace Insolvency.Common.Exceptions
{
    public class UnauthorizedHttpResponseException : HttpResponseException
    {
        public override int StatusCode => 403;

        public override string Message => "Unauthorized";
    }
}
