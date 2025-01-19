using Backend.Features.Suppliers;
using Backend.Service.IService;

namespace Backend.Service
{
	public class SuppliersService : ISuppliersService
	{
		private readonly BackendContext context;

		public SuppliersService(BackendContext context)
		{
			this.context = context;
		}

		/// <summary>
		/// Get Suppliers data from Db
		/// </summary>
		/// <param name="request"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<List<Supplier>> GetSuppliers(SupplierListQuery request, CancellationToken cancellationToken)
		{
			var query = context.Suppliers.AsQueryable();
			if (!string.IsNullOrEmpty(request.Name))
				query = query.Where(q => q.Name.ToLower().Contains(request.Name.ToLower()));

			return await query.OrderBy(q => q.Name).ToListAsync(cancellationToken);
			
		}
	}
}
