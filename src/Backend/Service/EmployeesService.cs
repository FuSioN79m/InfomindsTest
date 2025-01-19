using Backend.Features.Employees;
using Backend.Features.Suppliers;
using Backend.Service.IService;

namespace Backend.Service
{
	public class EmployeesService : IEmployeesService
	{
		private readonly BackendContext context;

		public EmployeesService(BackendContext context)
		{
			this.context = context;
		}

		/// <summary>
		/// Get Employees data from Db
		/// </summary>
		/// <param name="request"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<List<Employee>> GetEmployees(EmployeesListQuery request, CancellationToken cancellationToken)
		{
			//including Department table data to execute one single query
			var query = context.Employees.Include(t => t.Department).AsQueryable();
			if (!string.IsNullOrEmpty(request.FirstName))
				query = query.Where(q => q.FirstName.ToLower().Contains(request.FirstName.ToLower()));
			if (!string.IsNullOrEmpty(request.LastName))
				query = query.Where(q => q.LastName.ToLower().Contains(request.LastName.ToLower()));

			return await query.OrderBy(q => q.LastName).ThenBy(q => q.FirstName).ToListAsync(cancellationToken);
		}
	}
}
