namespace Insolvency.Models.OperationResults
{
    public class OperationResult<TResult>
    {
        public readonly TResult Value;
        public readonly OperationError OperationError;

        protected OperationResult(TResult value, OperationError error)
        {
            Value = value;
            OperationError = error;
        }

        public static OperationResult<TResult> Success(TResult value)
        {
            return new OperationResult<TResult>(value, null);
        }

        public static OperationResult<TResult> Error(TResult value, OperationError error)
        {
            return new OperationResult<TResult>(value, error);
        }

        public bool IsSuccess => OperationError == null;
        public bool IsError => OperationError != null;
    }
}
