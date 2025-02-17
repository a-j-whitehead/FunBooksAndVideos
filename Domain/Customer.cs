namespace Domain
{
    public class Customer
    {
        public int CustomerId { get; }
        public bool IsBookMember { get; private set; }
        public bool IsVideoMember { get; private set; }
        public IEnumerable<PurchaseOrder> PurchaseOrders { get; }

        public void ActivateBookMembership()
        {
            IsBookMember = true;
        }

        public void ActivateVideoMembership()
        {
            IsVideoMember = true;
        }
    }
}