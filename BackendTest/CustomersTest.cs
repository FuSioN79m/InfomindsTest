using Backend.Features.Customers;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTest
{
	public class CustomersTest
	{


		private readonly BackendContext _context;
		private readonly List<Customer> _testCustomers;
		private readonly List<CustomerCategory> _testCategories;

		public CustomersTest()
		{
			var options = new DbContextOptionsBuilder<BackendContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase_" + System.Guid.NewGuid())
				.Options;

			_context = new BackendContext(options);

			// Setup test categories
			_testCategories = new List<CustomerCategory>
			{
				new CustomerCategory { Id = 1, Code = "RETAIL", Description = "Retail Customer" },
				new CustomerCategory { Id = 2, Code = "CORP", Description = "Corporate Customer" }
			};

			// Setup test customers
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
					CustomerCategoryId = 2,
					CustomerCategory = _testCategories[1]
				},
				new Customer
				{
					Id = 2,
					Name = "John",
					Address = "via monte napoleone",
					Email = "john@shop.com",
					Phone = "0987654321",
					Iban = "IT60X0542811101000000654321",
					CustomerCategoryId = null,
					CustomerCategory = null
				},
				new Customer
				{
					Id = 3,
					Name = "Pippo Baudo",
					Address = "Via dei mille, Messina",
					Email = "pippo@baudo.com",
					Phone = "5555555555",
					Iban = "IT60X0542811101000000987654",
					CustomerCategoryId = 1,
					CustomerCategory = _testCategories[0]
				}
			};

			_context.CustomerCategories.AddRange(_testCategories);
			_context.Customers.AddRange(_testCustomers);
			_context.SaveChanges();
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}

		[Fact]
		public async Task Handle_NoFilter_ReturnsAllCustomers()
		{
			// Arrange
			var handler = new CustomersListQueryHandler(_context);
			var request = new CustomersListQuery();

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Equal(_testCustomers.Count, result.Count);
			Assert.Null(result[1].Category);

			// Verify ordering by Name
			Assert.Equal("Google Corporation", result[0].Name, ignoreCase: true);
			Assert.Equal("john", result[1].Name, ignoreCase: true);
			Assert.Equal("pippo baudo", result[2].Name, ignoreCase: true);
		}

		[Fact]
		public async Task Handle_WithNameFilter_ReturnsFilteredCustomers()
		{
			// Arrange
			var handler = new CustomersListQueryHandler(_context);
			var request = new CustomersListQuery { Name = "pippo" };

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			Assert.All(result, r => Assert.Contains("pippo", r.Name, StringComparison.OrdinalIgnoreCase));
		}

		[Fact]
		public async Task Handle_WithEmailFilter_ReturnsFilteredCustomers()
		{
			// Arrange
			var handler = new CustomersListQueryHandler(_context);
			var request = new CustomersListQuery { Email = "google" };

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			Assert.All(result, r => Assert.Contains("google", r.Email, StringComparison.OrdinalIgnoreCase));
		}

		[Fact]
		public async Task Handle_WithBothFilters_ReturnsFilteredCustomers()
		{
			// Arrange
			var handler = new CustomersListQueryHandler(_context);
			var request = new CustomersListQuery
			{
				Name = "google",
				Email = "google"
			};

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			Assert.Equal("google corporation", result[0].Name, ignoreCase: true);
			Assert.Equal("google@google.com", result[0].Email, ignoreCase: true);
		}

		[Fact]
		public async Task Handle_WithNonExistingName_ReturnsEmptyList()
		{
			// Arrange
			var handler = new CustomersListQueryHandler(_context);
			var request = new CustomersListQuery { Name = "NonExisting" };

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public async Task Handle_VerifyCategoryData_ReturnsCorrectCategoryInfo()
		{
			// Arrange
			var handler = new CustomersListQueryHandler(_context);
			var request = new CustomersListQuery();

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			var corporateCustomer = result.First(c => c.Name == "Google Corporation");
			Assert.NotNull(corporateCustomer.Category);
			Assert.Equal("CORP", corporateCustomer.Category.Code);
			Assert.Equal("Corporate Customer", corporateCustomer.Category.Description);

			var retailCustomer = result.First(c => c.Name == "Pippo Baudo");
			Assert.NotNull(retailCustomer.Category);
			Assert.Equal("RETAIL", retailCustomer.Category.Code);
			Assert.Equal("Retail Customer", retailCustomer.Category.Description);
		}

		[Fact]
		public async Task Handle_VerifyCustomerDetails_ReturnsCompleteCustomerInfo()
		{
			// Arrange
			var handler = new CustomersListQueryHandler(_context);
			var request = new CustomersListQuery { Name = "Pippo Baudo" };

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			var customer = result[0];
			Assert.Equal(3, customer.Id);
			Assert.Equal("Via dei mille, Messina", customer.Address);
			Assert.Equal("5555555555", customer.Phone);
			Assert.Equal("IT60X0542811101000000987654", customer.Iban);
			Assert.NotNull(customer.Category);
		}

	}
}
