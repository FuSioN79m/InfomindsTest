using Backend.Extensions;
using MediatR;

namespace Backend.Features.Suppliers;


public class SupplierListQuery : IRequest<List<SupplierListQueryResponseDto>>
{
    public string? Name { get; set; }
}

public class SupplierListQueryHandler(BackendContext context) : IRequestHandler<SupplierListQuery, List<SupplierListQueryResponseDto>>
{
    private readonly BackendContext context = context;

    public async Task<List<SupplierListQueryResponseDto>> Handle(SupplierListQuery request, CancellationToken cancellationToken)
    {
        var query = context.Suppliers.AsQueryable();
        if (!string.IsNullOrEmpty(request.Name))
            query = query.Where(q => q.Name.ToLower().Contains(request.Name.ToLower()));

        var data = await query.OrderBy(q => q.Name).ToListAsync(cancellationToken);
        var result = new List<SupplierListQueryResponseDto>();
        foreach (var item in data)
            result.Add(item.toSuppliersDto());//fill object using extension method

		//previous code
		/*
        foreach (var item in data)
        {
            var resultItem = new SupplierListQueryResponse
            {
                Id = item.Id,
                Name = item.Name,
                Address = item.Address,
                Email = item.Email,
                Phone = item.Phone,
            };
            
            result.Add(resultItem);
        }
        */

		return result;
    }
}