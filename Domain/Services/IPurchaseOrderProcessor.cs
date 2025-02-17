using Domain.Dtos;

namespace Domain.Services
{
    public interface IPurchaseOrderProcessor
    {
        Task Process(PurchaseOrderDto dto);
    }
}