using API.Contracts;
using API.Controller;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeController(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _employeeRepository.GetAll();
        if (!result.Any())
        {
            return NotFound("Data Not Found");
        }

        return Ok(result);
    }

    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        var result = _employeeRepository.GetByGuid(guid);
        if (result is null)
        {
            return NotFound("Id Not Found");
        }
        return Ok(result);
    }

    [HttpPost]
    public IActionResult Create(Employee employee)
    {
        var result = _employeeRepository.Create(employee);
        if (result is null)
        {
            return BadRequest("Failed to create data");
        }

        return Ok(result);
    }

    [HttpPut("{guid}")]
    public IActionResult Update(Guid guid, Employee employee)
    {
        var existingEmployee = _employeeRepository.GetByGuid(guid);
        if (existingEmployee == null)
        {
            return NotFound("Employee not found");
        }

        var result = _employeeRepository.Update(employee);
        if (!result)
        {
            return BadRequest("Failed to update data");
        }

        return Ok(result);
    }

    [HttpDelete("{guid}")]
    public IActionResult Delete(Guid guid)
    {
        var existingEmployee = _employeeRepository.GetByGuid(guid);
        if (existingEmployee == null)
        {
            return NotFound("Employee not found");
        }

        var result = _employeeRepository.Delete(existingEmployee);
        if (!result)
        {
            return BadRequest("Failed to delete data");
        }

        return Ok(result);
    }
}
