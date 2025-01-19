using Backend.Features.Customers;

namespace Backend.Extensions
{
	public static class CustomersExtension
	{
		public static CustomersListQueryResponseDto toCustomersDto(this Customer c)
		{
			return new CustomersListQueryResponseDto()
			{
				Id = c.Id,
				Name = c.Name,
				Address = c.Address,
				Email = c.Email,
				Phone = c.Phone,
				Iban = c.Iban,
				Category = c.CustomerCategory == null ? null : new CustomersListQueryResponseCategoryDto() {Code = c.CustomerCategory.Code, Description = c.CustomerCategory.Description},
			};
		}
	}
}
