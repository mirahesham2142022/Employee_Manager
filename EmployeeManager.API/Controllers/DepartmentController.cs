using EmployeeManager.Application.Dtos.DepartmentDtos;
using EmployeeManager.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {

        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET /api/departments/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken = default)
        {
            var departments = await _departmentService.GetAllAsync(cancellationToken);
            return Ok(departments);
        }

        // GET /api/departments/GetById/3
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var department = await _departmentService.GetByIdAsync(id, cancellationToken);

            if (department is null)
                return NotFound();

            return Ok(department);
        }

        // POST /api/departments/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(
            [FromBody] CreateDepartmentDto dto,
            CancellationToken cancellationToken = default)
        {
            var department = await _departmentService.CreateAsync(dto, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = department.Id },
                department);
        }

        // PUT /api/departments/Update/3
        [HttpPut("Update/{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateDepartmentDto dto,
            CancellationToken cancellationToken = default)
        {
            var department = await _departmentService.UpdateAsync(
                id, dto, cancellationToken);

            if (department is null)
                return NotFound();

            return Ok(department);
        }

        // DELETE /api/departments/Delete/3
        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken = default)
        {
            await _departmentService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
