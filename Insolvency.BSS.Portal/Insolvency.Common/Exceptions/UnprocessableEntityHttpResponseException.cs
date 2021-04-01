namespace Insolvency.Common.Exceptions
{
    public class UnprocessableEntityHttpResponseException : HttpResponseException
    {
        private readonly string _message = "Unprocessable Entity";

        public UnprocessableEntityHttpResponseException() { }

        public UnprocessableEntityHttpResponseException(string message)
        {
            _message = message;
        }

        public override int StatusCode => 422;

        public override string Message => _message;
    }
}
