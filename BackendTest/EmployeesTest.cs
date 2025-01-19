using Backend.Features.Employees;
using Backend.Infrastructure.Database;
using Backend.Service.IService;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BackendTest
{
	public class EmployeesTest
	{
		private readonly Mock<IEmployeesService> _employeesServiceMock;
		private readonly List<Employee> _testEmployees;

		public EmployeesTest()
		{
			_employeesServiceMock = new Mock<IEmployeesService>();

			// Setup test data
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
					Department = new Department
					{
						Id = 1,
						Code = "IT",
						Description = "Information Technology"
					}
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
					Department = new Department
					{
						Id = 2,
						Code = "HR",
						Description = "Human Resources"
					}
				}
			};
		}

		[Fact]
		public async Task Handle_NoFilter_ReturnsAllEmployees()
		{
			// Arrange
			var request = new EmployeesListQuery();
			_employeesServiceMock.Setup(x => x.GetEmployees(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(_testEmployees);

			var handler = new EmployeesListQueryHandler(_employeesServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Equal(_testEmployees.Count, result.Count);
			_employeesServiceMock.Verify(x => x.GetEmployees(request, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Handle_WithFirstNameFilter_CallsServiceWithCorrectParameters()
		{
			// Arrange
			var request = new EmployeesListQuery { FirstName = "Mike" };
			var filteredEmployees = _testEmployees.Where(e => e.FirstName.Contains("Mike")).ToList();

			_employeesServiceMock.Setup(x => x.GetEmployees(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(filteredEmployees);

			var handler = new EmployeesListQueryHandler(_employeesServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			Assert.Equal("Mike", result[0].FirstName);
			_employeesServiceMock.Verify(x => x.GetEmployees(
				It.Is<EmployeesListQuery>(q => q.FirstName == "Mike"),
				It.IsAny<CancellationToken>()),
				Times.Once);
		}

		[Fact]
		public async Task Handle_WithLastNameFilter_CallsServiceWithCorrectParameters()
		{
			// Arrange
			var request = new EmployeesListQuery { LastName = "Connor" };
			var filteredEmployees = _testEmployees.Where(e => e.LastName == "Connor").ToList();

			_employeesServiceMock.Setup(x => x.GetEmployees(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(filteredEmployees);

			var handler = new EmployeesListQueryHandler(_employeesServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			Assert.Equal("Connor", result[0].LastName);
			_employeesServiceMock.Verify(x => x.GetEmployees(
				It.Is<EmployeesListQuery>(q => q.LastName == "Connor"),
				It.IsAny<CancellationToken>()),
				Times.Once);
		}

		[Fact]
		public async Task Handle_WithBothFilters_CallsServiceWithCorrectParameters()
		{
			// Arrange
			var request = new EmployeesListQuery
			{
				FirstName = "John",
				LastName = "Connor"
			};

			var filteredEmployees = _testEmployees
				.Where(e => e.FirstName == "John" && e.LastName == "Connor")
				.ToList();

			_employeesServiceMock.Setup(x => x.GetEmployees(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(filteredEmployees);

			var handler = new EmployeesListQueryHandler(_employeesServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			Assert.Equal("John", result[0].FirstName);
			Assert.Equal("Connor", result[0].LastName);
			_employeesServiceMock.Verify(x => x.GetEmployees(request, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Handle_ServiceReturnsEmptyList_ReturnsEmptyResult()
		{
			// Arrange
			var request = new EmployeesListQuery { FirstName = "NonExisting" };
			_employeesServiceMock.Setup(x => x.GetEmployees(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<Employee>());

			var handler = new EmployeesListQueryHandler(_employeesServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Empty(result);
			_employeesServiceMock.Verify(x => x.GetEmployees(request, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Handle_VerifyDepartmentMapping_ReturnsMappedDepartmentInfo()
		{
			// Arrange
			var request = new EmployeesListQuery { FirstName = "Mike" };
			var employee = _testEmployees.First();

			_employeesServiceMock.Setup(x => x.GetEmployees(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<Employee> { employee });

			var handler = new EmployeesListQueryHandler(_employeesServiceMock.Object);

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.Single(result);
			var employeeDto = result[0];
			Assert.NotNull(employeeDto.Department);
			Assert.Equal("IT", employeeDto.Department.Code);
			Assert.Equal("Information Technology", employeeDto.Department.Description);
		}


	}
}
