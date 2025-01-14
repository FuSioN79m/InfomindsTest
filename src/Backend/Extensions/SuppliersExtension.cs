using Backend.Features.Suppliers;

namespace Backend.Extensions
{
	public static class SuppliersExtension
	{
		public static SupplierListQueryResponse toSuppliersDto(this Supplier c)
		{
			return new SupplierListQueryResponse()
			{
				Id = c.Id,
				Name = c.Name,
				Address = c.Address,
				Email = c.Email,
				Phone = c.Phone
			};
		}
	}
}
