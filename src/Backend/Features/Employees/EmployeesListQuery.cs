using Backend.Extensions;
using Backend.Service;
using Backend.Service.IService;

namespace Backend.Features.Employees;

public class EmployeesListQuery : IRequest<List<EmployeesListQueryResponseDto>>
{
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
}


public class EmployeesListQueryHandler(IEmployeesService employeesService) : IRequestHandler<EmployeesListQuery, List<EmployeesListQueryResponseDto>>
{
	private readonly IEmployeesService _employeesService = employeesService;

	public async Task<List<EmployeesListQueryResponseDto>> Handle(EmployeesListQuery request, CancellationToken cancellationToken)
	{
		//using service
		var data = await _employeesService.GetEmployees(request,cancellationToken);

		var result = new List<EmployeesListQueryResponseDto>();
		foreach (var item in data)
			result.Add(item.toEmployeesDto());//fill object using extension method

		////including Department table data to execute one single query
		//var query = context.Employees.Include(t => t.Department).AsQueryable();
		//if (!string.IsNullOrEmpty(request.FirstName))
		//	query = query.Where(q => q.FirstName.ToLower().Contains(request.FirstName.ToLower()));
		//if (!string.IsNullOrEmpty(request.LastName))
		//	query = query.Where(q => q.LastName.ToLower().Contains(request.LastName.ToLower()));

		//var data = await query.OrderBy(q => q.LastName).ThenBy(q => q.FirstName).ToListAsync(cancellationToken);


		//previous code that call db n times
		/*
		foreach (var item in data)
		{
			var resultItem = new EmployeesListQueryResponse
			{
				Id = item.Id,
				Code = item.Code,
				FirstName = item.FirstName,
				LastName = item.LastName,
				Address = item.Address,
				Email = item.Email,
				Phone = item.Phone,
			};

			var department = await context.Departments.SingleOrDefaultAsync(q => q.Id == item.DepartmentId, cancellationToken);
			if (department is not null)
				resultItem.Department = new EmployeesListQueryResponseDepartment
				{
					Code = department.Code,
					Description = department.Description
				};


			result.Add(resultItem);
		}
		*/
		return result;
	}
}