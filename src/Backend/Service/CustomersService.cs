using Backend.Features.Customers;
using Backend.Features.Employees;
using Backend.Features.Suppliers;
using Backend.Service.IService;

namespace Backend.Service
{
	public class CustomersService : ICustomersService
	{
		private readonly BackendContext context;

		public CustomersService(BackendContext context)
		{
			this.context = context;
		}

		/// <summary>
		/// Get Customers data from Db
		/// </summary>
		/// <param name="request"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<List<Customer>> GetCustomers(CustomersListQuery request, CancellationToken cancellationToken)
		{
			//including CustomerCategory table data to execute one single query
			var query = context.Customers.Include(t => t.CustomerCategory).AsQueryable();
			if (!string.IsNullOrEmpty(request.Name))
				query = query.Where(q => q.Name.ToLower().Contains(request.Name.ToLower()));
			if (!string.IsNullOrEmpty(request.Email))
				query = query.Where(q => q.Email.ToLower().Contains(request.Email.ToLower()));

			return await query.OrderBy(q => q.Name).ThenBy(q => q.Email).ToListAsync(cancellationToken);
		}
	}
}
