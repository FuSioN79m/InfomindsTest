using Backend.Features.Suppliers;
using Backend.Infrastructure.Database;
using Backend.Service.IService;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BackendTest
{
	public class SupplierListQueryHandlerTests
	{
		private readonly Mock<ISuppliersService> _suppliersServiceMock;
		private readonly List<Supplier> _testSuppliers;

		public SupplierListQueryHandlerTests()
		{
			_suppliersServiceMock = new Mock<ISuppliersService>();

			// Setup test data
			_testSuppliers = new List<Supplier>
			{
				new Supplier
				{
					Id = 1,
					Name = "ABC Suppliers",
					Address = "123 Supply St",
					Email = "contact@abcsuppliers.com",
					Phone = "1234567890"
				},
				new Supplier
				{
					Id = 2,
					Name = "XYZ Corporation",
					Address = "456 Corp Ave",
					Email = "info@xyzcorp.com",
					Phone = "0987654321"
				}
			};
		}

		[Fact]
		public async Task Handle_NoFilter_ReturnsAllSuppliers()
		{
			// Arrange
			var request = new SupplierListQuery();
			_suppliersServiceMock.Setup(x => x.GetSuppliers(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(_testSuppliers);

			var handler = new SupplierListQueryHandler(_suppliersServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Equal(_testSuppliers.Count, result.Count);
			_suppliersServiceMock.Verify(x => x.GetSuppliers(request, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Handle_WithNameFilter_CallsServiceWithCorrectParameters()
		{
			// Arrange
			var request = new SupplierListQuery { Name = "ABC" };
			var filteredSuppliers = _testSuppliers.Where(s => s.Name.Contains("ABC")).ToList();

			_suppliersServiceMock.Setup(x => x.GetSuppliers(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(filteredSuppliers);

			var handler = new SupplierListQueryHandler(_suppliersServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			Assert.Equal("ABC Suppliers", result[0].Name);
			_suppliersServiceMock.Verify(x => x.GetSuppliers(
				It.Is<SupplierListQuery>(q => q.Name == "ABC"),
				It.IsAny<CancellationToken>()),
				Times.Once);
		}

		[Fact]
		public async Task Handle_ServiceReturnsEmptyList_ReturnsEmptyResult()
		{
			// Arrange
			var request = new SupplierListQuery { Name = "NonExisting" };
			_suppliersServiceMock.Setup(x => x.GetSuppliers(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<Supplier>());

			var handler = new SupplierListQueryHandler(_suppliersServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Empty(result);
			_suppliersServiceMock.Verify(x => x.GetSuppliers(request, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Handle_VerifySupplierMapping_ReturnsMappedSupplierInfo()
		{
			// Arrange
			var request = new SupplierListQuery { Name = "ABC" };
			var supplier = _testSuppliers.First();

			_suppliersServiceMock.Setup(x => x.GetSuppliers(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<Supplier> { supplier });

			var handler = new SupplierListQueryHandler(_suppliersServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			var supplierDto = result[0];
			Assert.Equal(supplier.Id, supplierDto.Id);
			Assert.Equal(supplier.Name, supplierDto.Name);
			Assert.Equal(supplier.Address, supplierDto.Address);
			Assert.Equal(supplier.Email, supplierDto.Email);
			Assert.Equal(supplier.Phone, supplierDto.Phone);
		}

	}
}
