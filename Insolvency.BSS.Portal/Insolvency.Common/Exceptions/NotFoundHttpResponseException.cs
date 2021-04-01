namespace Insolvency.Common.Exceptions
{
    public class NotFoundHttpResponseException : HttpResponseException
    {
        public override int StatusCode => 404;

        public override string Message => "Not Found";
    }
}
