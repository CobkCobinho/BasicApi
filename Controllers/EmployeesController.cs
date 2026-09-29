using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("employees")]
public class EmployeesController : ControllerBase
{
    // "static" para que a lista sobreviva entre as requisições
    // (um novo Controller é criado a cada requisição)
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
        return Ok(_employees);
    }

    // GET /employees/{id}
    [HttpGet("{id}")]
    public ActionResult<Employee> GetById(int id)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    // POST /employees
    [HttpPost]
    public ActionResult<Employee> Create(Employee employee)
    {
        employee.Id = _nextId++;
        _employees.Add(employee);

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    // DELETE /employees/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
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
