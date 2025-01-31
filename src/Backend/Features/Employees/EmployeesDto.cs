﻿namespace Backend.Features.Employees
{
	public class EmployeesListQueryResponseDto
	{
		public int Id { get; set; }
		public string Code { get; internal set; } = "";
		public string FirstName { get; set; } = "";
		public string LastName { get; set; } = "";
		public string Address { get; set; } = "";
		public string Email { get; set; } = "";
		public string Phone { get; set; } = "";
		public EmployeesListQueryResponseDepartmentDto? Department { get; set; }
	}

	public class EmployeesListQueryResponseDepartmentDto
	{
		public string Code { get; set; } = "";
		public string Description { get; set; } = "";
	}
}
