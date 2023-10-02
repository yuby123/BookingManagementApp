/*using API.Contracts;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;

namespace API.Controllers
{
    public class GenericController<T> : ControllerBase where T : class
    {
        private readonly IRepository<T> _repository;

        public GenericController(IRepository<T> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _repository.GetAll();
            if (!result.Any())
            {
                return NotFound("Data Not Found");
            }

            return Ok(result);
        }

        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            var result = _repository.GetByGuid(guid);
            if (result is null)
            {
                return NotFound("Id Not Found");
            }
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create(T entity)
        {
            var result = _repository.Create(entity);
            if (result is null)
            {
                return BadRequest("Failed to create data");
            }

            return Ok(result);
        }



        [HttpPut("{guid}")]
        public IActionResult Update(Guid guid, [FromBody] T values)
        {
            var existingEntity = _repository.GetByGuid(guid);

            if (existingEntity == null)
            {
                return NotFound($"guid {typeof(T).Name} not found");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entityType = typeof(T);
            var properties = entityType.GetProperties();

            foreach (var property in properties)
            {
                if (property.Name != "Guid")
                {
                    var newValue = property.GetValue(values);
                    property.SetValue(existingEntity, newValue);
                }
            }

            var updateResult = _repository.Update(existingEntity);

            if (!updateResult)
            {
                return BadRequest($"Failed to update {typeof(T).Name.ToLower()}");
            }

            return Ok(existingEntity); 
        }



        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {

            var entity = _repository.GetByGuid(guid);
            if (entity is null)
            {
                return NotFound($"{typeof(T).Name} not found");
            }

            var result = _repository.Delete(entity);
            if (!result)
            {
                return NotFound($"{typeof(T).Name} not found");
            }

            return Ok(result);
        }
    }
}
*/