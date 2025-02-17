namespace Domain.Dtos
{
    public class ShippingSlipDto
    {
        public int CustomerId { get; set; }
        public IEnumerable<int> ProductIds { get; set; }

        public ShippingSlipDto(int customerId, IEnumerable<int> productIds)
        {
            CustomerId = customerId;
            ProductIds = productIds;
        }
    }
}
