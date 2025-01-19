using Backend.Features.Suppliers;

namespace Backend.Service.IService
{
	public interface ISuppliersService
	{
		Task<List<Supplier>> GetSuppliers(SupplierListQuery request, CancellationToken cancellationToken);
	}
}
