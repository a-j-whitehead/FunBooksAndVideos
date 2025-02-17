namespace Domain
{
    public class InvalidPurchaseOrderException : Exception
    {
        public InvalidPurchaseOrderException(string? message) : base(message)
        {

        }
    }
}
