using Domain.Dtos;

namespace Domain.Services
{
    public interface IShippingService
    {
        Task SendShippingSlip(ShippingSlipDto shippingSlipDto);
    }
}
