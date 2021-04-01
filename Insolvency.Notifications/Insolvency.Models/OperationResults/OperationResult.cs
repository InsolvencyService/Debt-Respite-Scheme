
namespace Insolvency.Models.OperationResults
{
    public class OperationResult : OperationResult<object>
    {
        protected OperationResult(object value, OperationError error)
            : base(value, error)
        {
        }

        public static OperationResult Success()
        {
            return new OperationResult(null, null);
        }

        public static OperationResult Error(OperationError error)
        {
            return new OperationResult(null, error);
        }       
    }
}
