using Backend.Features.Customers;
using Backend.Features.Employees;
using Backend.Features.Suppliers;

namespace Backend.Service.IService
{
	public interface ICustomersService
	{
		Task<List<Customer>> GetCustomers(CustomersListQuery request, CancellationToken cancellationToken);
	}
}
