namespace Insolvency.Portal.Models
{
    public class ModelFieldError
    {
        public string Field { get; set; }
        public string Error { get; set; }

        public ModelFieldError(string field, string error)
        {
            Field = field;
            Error = error;
        }
    }
}
