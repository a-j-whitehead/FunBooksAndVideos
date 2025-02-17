using Domain.Dtos;

namespace Domain.Services
{
    public class ShippingService : IShippingService
    {
        public Task SendShippingSlip(ShippingSlipDto shippingSlipDto)
        {
            // This is a placeholder for where this logic might go
            return Task.CompletedTask;
        }
    }
}
