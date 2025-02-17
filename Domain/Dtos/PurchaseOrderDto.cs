namespace Domain.Dtos
{
    public class PurchaseOrderDto
    {
        public int PurchaseOrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<string> ItemLines { get; set; }

        public PurchaseOrderDto(int purchaseOrderId, int customerId, decimal total, IEnumerable<string> itemLines)
        {
            PurchaseOrderId = purchaseOrderId;
            CustomerId = customerId;
            Total = total;
            ItemLines = itemLines;
        }
    }
}
