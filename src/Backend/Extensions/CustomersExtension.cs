using Backend.Features.Customers;

namespace Backend.Extensions
{
	public static class CustomersExtension
	{
		public static CustomersListQueryResponse toCustomersDto(this Customer c)
		{
			return new CustomersListQueryResponse()
			{
				Id = c.Id,
				Name = c.Name,
				Address = c.Address,
				Email = c.Email,
				Phone = c.Phone,
				Iban = c.Iban,
				Category = c.CustomerCategory == null ? null : new CustomersListQueryResponseCategory() {Code = c.CustomerCategory.Code, Description = c.CustomerCategory.Description},
			};
		}
	}
}
