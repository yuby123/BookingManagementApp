using API.Contracts;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller;
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;

    public RoleController(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _roleRepository.GetAll();
        if (!result.Any())
        {
            return NotFound("Data Not Found");
        }

        return Ok(result);
    }

    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        var result = _roleRepository.GetByGuid(guid);
        if (result is null)
        {
            return NotFound("Id Not Found");
        }
        return Ok(result);
    }

    [HttpPost]
    public IActionResult Create(Role role)
    {
        var result = _roleRepository.Create(role);
        if (result is null)
        {
            return BadRequest("Failed to create data");
        }

        return Ok(result);
    }

    [HttpPut("{guid}")]
    public IActionResult Update(Guid guid, Role role)
    {
        var existingRole = _roleRepository.GetByGuid(guid);
        if (existingRole == null)
        {
            return NotFound("Role not found");
        }

        var result = _roleRepository.Update(role);
        if (!result)
        {
            return BadRequest("Failed to update data");
        }

        return Ok(result);
    }

    [HttpDelete("{guid}")]
    public IActionResult Delete(Guid guid)
    {
        var existingRole = _roleRepository.GetByGuid(guid);
        if (existingRole == null)
        {
            return NotFound("Role not found");
        }

        var result = _roleRepository.Delete(existingRole);
        if (!result)
        {
            return BadRequest("Failed to delete data");
        }

        return Ok(result);
    }
}
