using Domain.Dtos;
using Domain.Infrastructure;

namespace Domain.Services
{
    public class PurchaseOrderProcessor : IPurchaseOrderProcessor
    {
        private readonly IRepository _repository;
        private readonly IShippingService _shippingService;

        public PurchaseOrderProcessor(IRepository repository, IShippingService shippingService)
        {
            _repository = repository;
            _shippingService = shippingService;
        }

        public async Task Process(PurchaseOrderDto dto)
        {
            if (!dto.ItemLines.Any())
            {
                throw new InvalidPurchaseOrderException("Cannot process empty purchase order");
            }

            var duplicatePurchaseOrder = await _repository.GetSingleOrDefault<PurchaseOrder>(x => x.PurchaseOrderId == dto.PurchaseOrderId);
            if (duplicatePurchaseOrder != null)
            {
                throw new InvalidPurchaseOrderException($"Duplicate purchase order with ID {dto.PurchaseOrderId}");
            }

            var customer = await _repository.GetSingleOrDefault<Customer>(x => x.CustomerId == dto.CustomerId);
            if (customer == null)
            {
                throw new InvalidPurchaseOrderException($"Customer with ID {dto.CustomerId} not found");
            }

            var products = await GetProductsCorrespondingToItemLines(dto, _repository);

            var purchaseOrder = new PurchaseOrder(dto.PurchaseOrderId, customer, dto.Total, products);
            await _repository.Add(purchaseOrder);

            ActivateCustomerMemberships(customer, products);

            var physicalProducts = products.Where(x => x.IsPhysicalProduct);
            if (physicalProducts.Any())
            {
                var shippingSlip = new ShippingSlipDto(customer.CustomerId, physicalProducts.Select(x => x.ProductId));
                await _shippingService.SendShippingSlip(shippingSlip);
            }

            await _repository.Save();
        }

        private static void ActivateCustomerMemberships(Customer customer, ICollection<Product> products)
        {
            if (products.Any(x => x.Type == ProductType.BookMembership))
            {
                customer.ActivateBookMembership();
            }
            if (products.Any(x => x.Type == ProductType.VideoMembership))
            {
                customer.ActivateVideoMembership();
            }
        }

        private static async Task<ICollection<Product>> GetProductsCorrespondingToItemLines(PurchaseOrderDto dto, IRepository repository)
        {
            var products = await repository.GetAll<Product>();

            var parsedItemLines = dto.ItemLines.Select(ParseItemLine).ToList();

            var productsCorrespondingToItemLines = parsedItemLines.Select(x => products.SingleOrDefault(y => x.Name == y.Name && x.Type == y.Type)).ToList();

            if (productsCorrespondingToItemLines.Any(x => x == null))
            {
                var missingProducts = parsedItemLines.Where(x => !products.Any(y => x.Name == y.Name && x.Type == y.Type)).ToList();
                throw new InvalidPurchaseOrderException($"Products not found: {string.Join(" | ", missingProducts.Select(x => x.Name))}");
            }

            return productsCorrespondingToItemLines!;
        }

        private static (ProductType Type, string Name) ParseItemLine(string itemLine)
        {
            if (itemLine == "Book Club Membership")
            {
                return (ProductType.BookMembership, itemLine);
            }
            else if (itemLine == "Video Club Membership")
            {
                return (ProductType.VideoMembership, itemLine);
            }
            else if (itemLine.StartsWith("Video \"") && itemLine.EndsWith('\"'))
            {
                return (ProductType.Video, itemLine[7..^1]);
            }
            else if (itemLine.StartsWith("Book \"") && itemLine.EndsWith('\"'))
            {
                return (ProductType.Book, itemLine[6..^1]);
            }
            else
            {
                throw new InvalidPurchaseOrderException($"Could not parse line item {itemLine}");
            }
        }
    }
}
