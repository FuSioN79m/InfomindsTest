using Backend.Features.Customers;
using Backend.Infrastructure.Database;
using Backend.Service;
using Backend.Service.IService;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTest
{
	public class CustomersTest
	{

		private readonly Mock<ICustomersService> _customersServiceMock;
		private readonly List<Customer> _testCustomers;

		public CustomersTest()
		{
			_customersServiceMock = new Mock<ICustomersService>();

			// Setup test data
			_testCustomers = new List<Customer>
			{
				new Customer
				{
					Id = 1,
					Name = "Google Corporation",
					Address = "somewhere around the world",
					Email = "google@google.com",
					Phone = "1234567890",
					Iban = "IT60X0542811101000000123456",
					CustomerCategory = new CustomerCategory
					{
						Id = 2,
						Code = "CORP",
						Description = "Corporate Customer"
					}
				},
				new Customer
				{
					Id = 2,
					Name = "Pippo Baudo",
					Address = "Via dei mille, Messina",
					Email = "pippo@baudo.com",
					Phone = "5555555555",
					Iban = "IT60X0542811101000000987654",
					CustomerCategory = new CustomerCategory
					{
						Id = 1,
						Code = "RETAIL",
						Description = "Retail Customer"
					}
				}
			};
		}

		[Fact]
		public async Task Handle_NoFilter_ReturnsAllCustomers()
		{
			// Arrange
			var request = new CustomersListQuery();
			_customersServiceMock.Setup(x => x.GetCustomers(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(_testCustomers);

			var handler = new CustomersListQueryHandler(_customersServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Equal(_testCustomers.Count, result.Count);
			_customersServiceMock.Verify(x => x.GetCustomers(request, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Handle_WithNameFilter_CallsServiceWithCorrectParameters()
		{
			// Arrange
			var request = new CustomersListQuery { Name = "Google" };
			var filteredCustomers = _testCustomers.Where(c => c.Name.Contains("Google")).ToList();

			_customersServiceMock.Setup(x => x.GetCustomers(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(filteredCustomers);

			var handler = new CustomersListQueryHandler(_customersServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			Assert.Equal("Google Corporation", result[0].Name);
			_customersServiceMock.Verify(x => x.GetCustomers(
				It.Is<CustomersListQuery>(q => q.Name == "Google"),
				It.IsAny<CancellationToken>()),
				Times.Once);
		}

		[Fact]
		public async Task Handle_WithEmailFilter_CallsServiceWithCorrectParameters()
		{
			// Arrange
			var request = new CustomersListQuery { Email = "google@google.com" };
			var filteredCustomers = _testCustomers.Where(c => c.Email == "google@google.com").ToList();

			_customersServiceMock.Setup(x => x.GetCustomers(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(filteredCustomers);

			var handler = new CustomersListQueryHandler(_customersServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			Assert.Equal("google@google.com", result[0].Email);
			_customersServiceMock.Verify(x => x.GetCustomers(
				It.Is<CustomersListQuery>(q => q.Email == "google@google.com"),
				It.IsAny<CancellationToken>()),
				Times.Once);
		}

		[Fact]
		public async Task Handle_WithBothFilters_CallsServiceWithCorrectParameters()
		{
			// Arrange
			var request = new CustomersListQuery
			{
				Name = "Google Corporation",
				Email = "google@google.com"
			};

			var filteredCustomers = _testCustomers
				.Where(c => c.Name.Contains("Google") && c.Email == "google@google.com")
				.ToList();

			_customersServiceMock.Setup(x => x.GetCustomers(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(filteredCustomers);

			var handler = new CustomersListQueryHandler(_customersServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			Assert.Equal("Google Corporation", result[0].Name);
			Assert.Equal("google@google.com", result[0].Email);
			_customersServiceMock.Verify(x => x.GetCustomers(request, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Handle_ServiceReturnsEmptyList_ReturnsEmptyResult()
		{
			// Arrange
			var request = new CustomersListQuery { Name = "NonExisting" };
			_customersServiceMock.Setup(x => x.GetCustomers(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<Customer>());

			var handler = new CustomersListQueryHandler(_customersServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Empty(result);
			_customersServiceMock.Verify(x => x.GetCustomers(request, It.IsAny<CancellationToken>()), Times.Once);
		}

	}
}