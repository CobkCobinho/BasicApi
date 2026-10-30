using EmployeeApi.Contracts;
using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("employees")]
public class EmployeesController : ControllerBase
{
    // O estado é compartilhado entre requisições e protegido pelo mesmo lock.
    private static readonly object _sync = new();
    private static readonly List<Employee> _employees = new()
    {
        new Employee { Id = 1, Name = "Ana Souza", Position = "Desenvolvedora", Salary = 8000m },
        new Employee { Id = 2, Name = "Carlos Lima", Position = "Analista de QA", Salary = 6500m }
    };

    private static int _nextId = 3;

    // GET /employees
    [HttpGet]
    public ActionResult<List<Employee>> GetAll()
    {
        lock (_sync)
        {
            // A serialização acontece depois da ação: devolva uma cópia da lista.
            return Ok(_employees.ToList());
        }
    }

    // GET /employees/{id}
    [HttpGet("{id}")]
    public ActionResult<Employee> GetById(int id)
    {
        lock (_sync)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            return employee == null ? NotFound() : Ok(employee);
        }
    }

    // POST /employees
    [HttpPost]
    public ActionResult<Employee> Create(CreateEmployeeRequest request)
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

            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }
    }

    // DELETE /employees/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        lock (_sync)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            _employees.Remove(employee);
            return NoContent();
        }
    }
}
