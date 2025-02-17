using Domain;
using Domain.Dtos;
using Domain.Infrastructure;
using Domain.Services;
using Moq;
using System.Linq.Expressions;

namespace Tests
{
    public class PurchaseOrderProcessorTests
    {
        private static readonly string _videoName = "Comprehensive First Aid Training";
        private static readonly string _bookName = "The Girl on the train";
        private static readonly string _bookMembership = "Book Club Membership";
        private static readonly string _videoMembership = "Video Club Membership";

        [Fact]
        public async Task Should_Generate_Shipping_Slip_For_Video()
        {
            var customer = new Customer();
            var purchaseOrder = new PurchaseOrderDto(45, 542, 6000m, [$"Video \"{_videoName}\""]);

            var repositoryMock = SetUpRepositoryMock(customer);
            var shippingServiceMock = new Mock<IShippingService>();

            var purchaseOrderProcessor = new PurchaseOrderProcessor(repositoryMock.Object, shippingServiceMock.Object);

            await purchaseOrderProcessor.Process(purchaseOrder);

            repositoryMock.Verify(x => x.Add(It.Is<PurchaseOrder>(x => x.Products.Single().Name == _videoName)), Times.Once());
            shippingServiceMock.Verify(x => x.SendShippingSlip(It.Is<ShippingSlipDto>(x => x.ProductIds.Count() == 1)), Times.Once());
        }

        [Fact]
        public async Task Should_Generate_Shipping_Slip_For_Book()
        {
            var customer = new Customer();
            var purchaseOrder = new PurchaseOrderDto(45, 234, 6000m, [$"Book \"{_bookName}\""]);

            var repositoryMock = SetUpRepositoryMock(customer);
            var shippingServiceMock = new Mock<IShippingService>();

            var purchaseOrderProcessor = new PurchaseOrderProcessor(repositoryMock.Object, shippingServiceMock.Object);

            await purchaseOrderProcessor.Process(purchaseOrder);

            repositoryMock.Verify(x => x.Add(It.Is<PurchaseOrder>(x => x.Products.Single().Name == _bookName)), Times.Once());
            shippingServiceMock.Verify(x => x.SendShippingSlip(It.Is<ShippingSlipDto>(x => x.ProductIds.Count() == 1)), Times.Once());
        }

        [Fact]
        public async void Should_Activate_Book_Membership()
        {
            var customer = new Customer();
            var purchaseOrder = new PurchaseOrderDto(45, 642, 6000m, [_bookMembership]);

            var repositoryMock = SetUpRepositoryMock(customer);
            var shippingServiceMock = new Mock<IShippingService>();

            var purchaseOrderProcessor = new PurchaseOrderProcessor(repositoryMock.Object, shippingServiceMock.Object);

            await purchaseOrderProcessor.Process(purchaseOrder);

            Assert.True(customer.IsBookMember);
            Assert.False(customer.IsVideoMember);
            shippingServiceMock.Verify(x => x.SendShippingSlip(It.IsAny<ShippingSlipDto>()), Times.Never());
        }

        [Fact]
        public async void Should_Activate_Video_Membership()
        {
            var customer = new Customer();
            var purchaseOrder = new PurchaseOrderDto(45, 42, 6000m, [_videoMembership]);

            var repositoryMock = SetUpRepositoryMock(customer);
            var shippingServiceMock = new Mock<IShippingService>();

            var purchaseOrderProcessor = new PurchaseOrderProcessor(repositoryMock.Object, shippingServiceMock.Object);

            await purchaseOrderProcessor.Process(purchaseOrder);

            Assert.True(customer.IsVideoMember);
            Assert.False(customer.IsBookMember);
            shippingServiceMock.Verify(x => x.SendShippingSlip(It.IsAny<ShippingSlipDto>()), Times.Never());
        }

        [Fact]
        public async void Should_Not_Process_Invalid_Item()
        {
            var customer = new Customer();
            var purchaseOrder = new PurchaseOrderDto(45, 54, 6000m, ["Invalid Item Description"]);

            var repositoryMock = SetUpRepositoryMock(customer);
            var shippingServiceMock = new Mock<IShippingService>();

            var purchaseOrderProcessor = new PurchaseOrderProcessor(repositoryMock.Object, shippingServiceMock.Object);

            try
            {
                await purchaseOrderProcessor.Process(purchaseOrder);
            }
            catch (InvalidPurchaseOrderException)
            {
                repositoryMock.Verify(x => x.Save(), Times.Never());
                return;
            }

            Assert.Fail("Expecting InvalidPurchaseOrderException due to invalid item");
        }

        [Fact]
        public async void Should_Not_Process_Duplicate_Purchase_Order()
        {
            var customer = new Customer();
            var purchaseOrder = new PurchaseOrderDto(45, 65, 6000m, [$"Book \"{_bookName}\""]);

            var repositoryMock = SetUpRepositoryMock(customer);
            repositoryMock.Setup(x => x.GetSingleOrDefault(It.IsAny<Expression<Func<PurchaseOrder, bool>>>()))
                .Returns(Task.FromResult<PurchaseOrder?>(new PurchaseOrder(45, customer, 6000m, [new Product(_bookName, ProductType.Book)])));
            var shippingServiceMock = new Mock<IShippingService>();

            var purchaseOrderProcessor = new PurchaseOrderProcessor(repositoryMock.Object, shippingServiceMock.Object);

            try
            {
                await purchaseOrderProcessor.Process(purchaseOrder);
            }
            catch (InvalidPurchaseOrderException)
            {
                repositoryMock.Verify(x => x.Save(), Times.Never());
                return;
            }

            Assert.Fail("Expecting InvalidPurchaseOrderException due to duplicate purchase order ID");
        }

        [Fact]
        public async void Should_Not_Process_Empty_Purchase_Order()
        {
            var customer = new Customer();
            var purchaseOrder = new PurchaseOrderDto(876, 542, 1200m, []);

            var repositoryMock = new Mock<IRepository>();
            repositoryMock.Setup(x => x.GetSingleOrDefault(It.IsAny<Expression<Func<Customer, bool>>>()))
                .Returns(Task.FromResult<Customer?>(customer));
            var shippingServiceMock = new Mock<IShippingService>();

            var purchaseOrderProcessor = new PurchaseOrderProcessor(repositoryMock.Object, shippingServiceMock.Object);

            try
            {
                await purchaseOrderProcessor.Process(purchaseOrder);
            }
            catch (InvalidPurchaseOrderException)
            {
                repositoryMock.Verify(x => x.Save(), Times.Never());
                return;
            }

            Assert.Fail("Expecting InvalidPurchaseOrderException due to empty purchase order");
        }

        [Fact]
        public async void Should_Process_Duplicate_Items_In_Purchase_Order()
        {
            var customer = new Customer();
            var purchaseOrder = new PurchaseOrderDto(45, 56, 6000m, [$"Video \"{_videoName}\"", $"Video \"{_videoName}\""]);

            var repositoryMock = SetUpRepositoryMock(customer);
            var shippingServiceMock = new Mock<IShippingService>();

            var purchaseOrderProcessor = new PurchaseOrderProcessor(repositoryMock.Object, shippingServiceMock.Object);

            await purchaseOrderProcessor.Process(purchaseOrder);

            shippingServiceMock.Verify(x => x.SendShippingSlip(It.Is<ShippingSlipDto>(x => x.ProductIds.Count() == 2)), Times.Once());
            Assert.False(customer.IsBookMember);
            Assert.False(customer.IsVideoMember);
        }

        private static Mock<IRepository> SetUpRepositoryMock(Customer customer)
        {
            var repositoryMock = new Mock<IRepository>();
            repositoryMock.Setup(x => x.GetSingleOrDefault(It.IsAny<Expression<Func<Customer, bool>>>()))
                .Returns(Task.FromResult<Customer?>(customer));
            repositoryMock
                .Setup(x => x.GetAll<Product>())
                .Returns(Task.FromResult<ICollection<Product>>(
                [
                    new Product(_videoName, ProductType.Video),
                    new Product(_bookName, ProductType.Book),
                    new Product(_videoMembership, ProductType.VideoMembership),
                    new Product(_bookMembership, ProductType.BookMembership),
                ]));
            return repositoryMock;
        }
    }
}