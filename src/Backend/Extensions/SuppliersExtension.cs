using Backend.Features.Suppliers;

namespace Backend.Extensions
{
	public static class SuppliersExtension
	{
		public static SupplierListQueryResponseDto toSuppliersDto(this Supplier c)
		{
			return new SupplierListQueryResponseDto()
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
