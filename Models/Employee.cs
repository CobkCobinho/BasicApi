namespace EmployeeApi.Models;

public class Employee
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public decimal Salary { get; init; }
}
