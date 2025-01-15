using Backend.Features.Employees;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BackendTest
{
	public class EmployeesTest
	{
		
        private readonly BackendContext _context;
		private readonly List<Employee> _testEmployees;
		private readonly List<Department> _testDepartments;

		public EmployeesTest()
		{
			var options = new DbContextOptionsBuilder<BackendContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase_" + System.Guid.NewGuid())
				.Options;

			_context = new BackendContext(options);

			// Setup test departments
			_testDepartments = new List<Department>
			{
				new Department { Id = 1, Code = "IT", Description = "Information Technology" },
				new Department { Id = 2, Code = "HR", Description = "Human Resources" }
			};

			// Setup test employees
			_testEmployees = new List<Employee>
			{
				new Employee
				{
					Id = 1,
					Code = "EMP001",
					FirstName = "Mike",
					LastName = "Test",
					Address = "123 Main St",
					Email = "mike.test@test.com",
					Phone = "987654321",
					DepartmentId = 1,
					Department = _testDepartments[0]
				},
				new Employee
				{
					Id = 2,
					Code = "EMP002",
					FirstName = "John",
					LastName = "Connor",
					Address = "456 Oak Ave",
					Email = "john.Connor@test.com",
					Phone = "089556677",
					DepartmentId = null,
					Department = null
				},
				new Employee
				{
					Id = 3,
					Code = "EMP003",
					FirstName = "Sarah",
					LastName = "Connor",
					Address = "789 Pine St",
					Email = "sarah.connor@test.com",
					Phone = "123456789",
					DepartmentId = 1,
					Department = _testDepartments[1]
				}
			};

			_context.Departments.AddRange(_testDepartments);
			_context.Employees.AddRange(_testEmployees);
			_context.SaveChanges();
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}

		[Fact]
		public async Task Handle_NoFilter_ReturnsAllEmployees()
		{
			// Arrange
			var handler = new EmployeesListQueryHandler(_context);
			var request = new EmployeesListQuery();

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Equal(_testEmployees.Count, result.Count);

			// Verify ordering (by LastName, then FirstName)
			Assert.Equal("Connor", result[0].LastName);
			Assert.Equal("Connor", result[1].LastName);
			Assert.Equal("Test", result[2].LastName);
			Assert.Equal("Sarah", result[1].FirstName); // Jane Smith should come before John Smith
		}

		[Fact]
		public async Task Handle_WithFirstNameFilter_ReturnsFilteredEmployees()
		{
			// Arrange
			var handler = new EmployeesListQueryHandler(_context);
			var request = new EmployeesListQuery { FirstName = "john" };

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			Assert.All(result, r => Assert.Contains("John", r.FirstName, StringComparison.OrdinalIgnoreCase));
		}

		[Fact]
		public async Task Handle_WithLastNameFilter_ReturnsFilteredEmployees()
		{
			// Arrange
			var handler = new EmployeesListQueryHandler(_context);
			var request = new EmployeesListQuery { LastName = "connor" };

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Equal(2, result.Count);
			Assert.All(result, r => Assert.Contains("connor", r.LastName, StringComparison.OrdinalIgnoreCase));
		}

		[Fact]
		public async Task Handle_WithBothFilters_ReturnsFilteredEmployees()
		{
			// Arrange
			var handler = new EmployeesListQueryHandler(_context);
			var request = new EmployeesListQuery
			{
				FirstName = "john",
				LastName = "connor"
			};

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			Assert.Equal("john", result[0].FirstName, ignoreCase: true);
			Assert.Equal("connor", result[0].LastName, ignoreCase: true);
		}

		[Fact]
		public async Task Handle_WithNonExistingName_ReturnsEmptyList()
		{
			// Arrange
			var handler = new EmployeesListQueryHandler(_context);
			var request = new EmployeesListQuery { FirstName = "NonExisting" };

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public async Task Handle_VerifyDepartmentData_ReturnsCorrectDepartmentInfo()
		{
			// Arrange
			var handler = new EmployeesListQueryHandler(_context);
			var request = new EmployeesListQuery();

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			var itEmployee = result.First(e => e.Code == "EMP001");
			Assert.NotNull(itEmployee.Department);
			Assert.Equal("IT", itEmployee.Department.Code);
			Assert.Equal("Information Technology", itEmployee.Department.Description);

			var hrEmployee = result.First(e => e.Code == "EMP002");
			Assert.Null(hrEmployee.Department);
			
		}
	}
}
