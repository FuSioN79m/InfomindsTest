using Backend.Features.Suppliers;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BackendTest
{
	public class SupplierListQueryHandlerTests
	{
		private readonly BackendContext _context;
		private readonly List<Supplier> _testSuppliers;

		public SupplierListQueryHandlerTests()
		{
			var options = new DbContextOptionsBuilder<BackendContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase_" + System.Guid.NewGuid())
				.Options;

			_context = new BackendContext(options);

			_testSuppliers = new List<Supplier>
		{
			new Supplier { Id = 1, Name = "Supplier A", Address = "Address A", Email = "a@test.com", Phone = "1111" },
			new Supplier { Id = 2, Name = "Supplier B", Address = "Address B", Email = "b@test.com", Phone = "2222" },
			new Supplier { Id = 3, Name = "Different Name", Address = "Address C", Email = "c@test.com", Phone = "3333" }
		};

			_context.Suppliers.AddRange(_testSuppliers);
			_context.SaveChanges();
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}

		[Fact]
		public async Task Handle_NoFilter_ReturnsAllSuppliers()
		{
			// Arrange
			var handler = new SupplierListQueryHandler(_context);
			var request = new SupplierListQuery { Name = null };

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Equal(_testSuppliers.Count, result.Count);
		}

		[Fact]
		public async Task Handle_WithNameFilter_ReturnsFilteredSuppliers()
		{
			// Arrange
			var handler = new SupplierListQueryHandler(_context);
			var request = new SupplierListQuery { Name = "supplier" };

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Equal(2, result.Count);
			Assert.All(result, item => Assert.Contains("Supplier", item.Name, StringComparison.OrdinalIgnoreCase));
		}

		[Fact]
		public async Task Handle_WithNonExistingName_ReturnsEmptyList()
		{
			// Arrange
			var handler = new SupplierListQueryHandler(_context);
			var request = new SupplierListQuery { Name = "NonExisting" };

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Empty(result);
		}
	}
}
