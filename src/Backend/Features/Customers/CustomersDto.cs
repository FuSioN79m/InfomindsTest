namespace Backend.Features.Customers
{
	public class CustomersListQueryResponseDto
	{

		public int Id { get; set; }
		public string Name { get; set; } = "";
		public string Address { get; set; } = "";
		public string Email { get; set; } = "";
		public string Phone { get; set; } = "";
		public string Iban { get; set; } = "";
		public CustomersListQueryResponseCategoryDto? Category { get; set; }
	}


	public class CustomersListQueryResponseCategoryDto
	{
		public string Code { get; set; } = "";
		public string Description { get; set; } = "";
	}
}
