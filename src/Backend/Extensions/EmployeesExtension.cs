using Backend.Features.Employees;

namespace Backend.Extensions
{
	public static class EmployeesExtension
	{
		public static EmployeesListQueryResponseDto toEmployeesDto(this Employee c)
		{
			return new EmployeesListQueryResponseDto()
			{
				Id = c.Id,
				Address = c.Address,
				Code = c.Code,
				Department = c.Department == null ? null : new EmployeesListQueryResponseDepartmentDto() { Code =  c.Department.Code, Description = c.Department.Description },
				Email = c.Email,
				FirstName = c.FirstName,
				LastName = c.LastName,
				Phone = c.Phone			

			};
		}
	}
}
/*
     public int Id { get; set; }
    public string Code { get; internal set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Address { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public EmployeesListQueryResponseDepartment? Department { get; set; }
 */