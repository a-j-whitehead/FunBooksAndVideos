using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Product
    {
        public int ProductId { get; }
        [StringLength(120)]
        public string Name { get; }
        public ProductType Type { get; }
        public bool IsPhysicalProduct => Type == ProductType.Book || Type == ProductType.Video;
        public IEnumerable<PurchaseOrder> PurchaseOrders { get; }

        protected Product() { }

        public Product(string name, ProductType type)
        {
            Name = name;
            Type = type;
            PurchaseOrders = new List<PurchaseOrder>();
        }
    }
}
