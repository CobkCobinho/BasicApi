using EmployeeApi.Contracts;
using EmployeeApi.Models;

namespace EmployeeApi;

public sealed class EmployeeStore
{
    private readonly object _sync = new();
    private readonly List<Employee> _employees =
    [
        new() { Id = 1, Name = "Ana Souza", Position = "Desenvolvedora", Salary = 8000m },
        new() { Id = 2, Name = "Carlos Lima", Position = "Analista de QA", Salary = 6500m }
    ];
    private int _nextId = 3;

    public List<Employee> GetAll()
    {
        lock (_sync) return _employees.ToList();
    }

    public Employee? GetById(int id)
    {
        lock (_sync) return _employees.FirstOrDefault(employee => employee.Id == id);
    }

    // O controller valida o contrato antes de chamar o armazenamento.
    public Employee Create(CreateEmployeeRequest request)
    {
        lock (_sync)
        {
            var employee = new Employee
            {
                Id = _nextId++,
                Name = request.Name.Trim(),
                Position = request.Position.Trim(),
                Salary = request.Salary!.Value
            };
            _employees.Add(employee);
            return employee;
        }
    }

    public bool Delete(int id)
    {
        lock (_sync)
        {
            var employee = _employees.FirstOrDefault(employee => employee.Id == id);
            return employee is not null && _employees.Remove(employee);
        }
    }
}
