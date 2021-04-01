namespace Insolvency.Common.Exceptions
{
    public class ConflictHttpResponseException : HttpResponseException
    {
        public override int StatusCode => 409;

        public override string Message => "Conflict";
    }
}
