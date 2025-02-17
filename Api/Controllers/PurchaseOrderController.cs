using Domain;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderProcessor _purchaseOrderProcessor;
        private readonly ILogger<PurchaseOrderController> _logger;

        public PurchaseOrderController(IPurchaseOrderProcessor purchaseOrderProcessor, ILogger<PurchaseOrderController> logger)
        {
            _purchaseOrderProcessor = purchaseOrderProcessor;
            _logger = logger;
        }

        [HttpPut(Name = "AddPurchaseOrder")]
        public async Task<IActionResult> AddPurchaseOrder(Domain.Dtos.PurchaseOrderDto dto)
        {
            try
            {
                await _purchaseOrderProcessor.Process(dto);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown while trying to add purchase order {PurchaseOrderId}", dto.PurchaseOrderId);

                if (ex is InvalidPurchaseOrderException)
                {
                    return BadRequest(ex.Message);
                }
                
                return Problem("Unexpected problem");
            }
        }
    }
}
