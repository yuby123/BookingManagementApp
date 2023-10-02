using API.Contracts;
using API.Controller;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller;


[ApiController]
[Route("api/[controller]")]
public class EducationController : ControllerBase
{
    private readonly IEducationRepository _educationRepository;

    public EducationController(IEducationRepository educationRepository)
    {
        _educationRepository = educationRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _educationRepository.GetAll();
        if (!result.Any())
        {
            return NotFound("Data Not Found");
        }

        return Ok(result);
    }

    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        var result = _educationRepository.GetByGuid(guid);
        if (result is null)
        {
            return NotFound("Id Not Found");
        }
        return Ok(result);
    }

    [HttpPost]
    public IActionResult Create(Education education)
    {
        var result = _educationRepository.Create(education);
        if (result is null)
        {
            return BadRequest("Failed to create data");
        }

        return Ok(result);
    }

    [HttpPut("{guid}")]
    public IActionResult Update(Guid guid, Education education)
    {
        var existingEducation = _educationRepository.GetByGuid(guid);
        if (existingEducation == null)
        {
            return NotFound("Education not found");
        }

        var result = _educationRepository.Update(education);
        if (!result)
        {
            return BadRequest("Failed to update data");
        }

        return Ok(result);
    }

    [HttpDelete("{guid}")]
    public IActionResult Delete(Guid guid)
    {
        var existingEducation = _educationRepository.GetByGuid(guid);
        if (existingEducation == null)
        {
            return NotFound("Education not found");
        }

        var result = _educationRepository.Delete(existingEducation);
        if (!result)
        {
            return BadRequest("Failed to delete data");
        }

        return Ok(result);
    }
}


