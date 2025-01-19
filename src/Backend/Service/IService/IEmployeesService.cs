using Backend.Features.Employees;
using Backend.Features.Suppliers;

namespace Backend.Service.IService
{
	public interface IEmployeesService
	{
		Task<List<Employee>> GetEmployees(EmployeesListQuery request, CancellationToken cancellationToken);
	}
}
