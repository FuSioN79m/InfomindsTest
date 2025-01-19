using Backend.Extensions;
using System.Runtime.CompilerServices;

namespace Backend.Features.Customers;

public class CustomersListQuery : IRequest<List<CustomersListQueryResponseDto>>
{
	public string? Name { get; set; }
	public string? Email { get; set; }
}



public class CustomersListQueryHandler : IRequestHandler<CustomersListQuery, List<CustomersListQueryResponseDto>>
{
	private readonly BackendContext context;

	public CustomersListQueryHandler(BackendContext context)
	{
		this.context = context;
	}

	public async Task<List<CustomersListQueryResponseDto>> Handle(CustomersListQuery request, CancellationToken cancellationToken)
	{
		//including CustomerCategory table data to execute one single query
		var query = context.Customers.Include(t=>t.CustomerCategory).AsQueryable();
		if (!string.IsNullOrEmpty(request.Name))
			query = query.Where(q => q.Name.ToLower().Contains(request.Name.ToLower()));
		if (!string.IsNullOrEmpty(request.Email))
			query = query.Where(q => q.Email.ToLower().Contains(request.Email.ToLower()));

		var data = await query.OrderBy(q => q.Name).ThenBy(q => q.Email).ToListAsync(cancellationToken);
		var result = new List<CustomersListQueryResponseDto>();
		foreach (var item in data)
			result.Add(item.toCustomersDto());//fill object using extension method

		//previous code that call db n times
		//foreach (var item in data)
		//{
		//    var resultItem = new CustomersListQueryResponse
		//    {
		//        Id = item.Id,
		//        Name = item.Name,
		//        Address = item.Address,
		//        Email = item.Email,
		//        Phone = item.Phone,
		//        Iban = item.Iban,
		//        Category = await context.CustomerCategories
		//            .Where(q => q.Id == item.CustomerCategoryId)
		//            .Select(q => new CustomersListQueryResponseCategory { Id = q.Id, Code = q.Code, Description = q.Description })
		//            .SingleOrDefaultAsync(cancellationToken),
		//    };

		//    result.Add(resultItem);
		//}

		return result;
	}
}