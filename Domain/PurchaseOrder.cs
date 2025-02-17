using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class PurchaseOrder
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PurchaseOrderId { get; set; }
        public Customer Customer { get; }
        public decimal Total { get; }
        public IEnumerable<Product> Products { get; }

        protected PurchaseOrder() { }

        public PurchaseOrder(int purchaseOrderId, Customer customer, decimal total, IEnumerable<Product> products)
        {
            PurchaseOrderId = purchaseOrderId;
            Customer = customer;
            Total = total;
            Products = products;
        }
    }
}
