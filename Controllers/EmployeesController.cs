using EmployeeApi.Contracts;
using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("employees")]
public class EmployeesController(EmployeeStore employees) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Employee>> GetAll() => Ok(employees.GetAll());

    [HttpGet("{id}")]
    public ActionResult<Employee> GetById(int id)
    {
        var employee = employees.GetById(id);
        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    public ActionResult<Employee> Create(CreateEmployeeRequest request)
    {
        var employee = employees.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id) => employees.Delete(id) ? NoContent() : NotFound();
}
