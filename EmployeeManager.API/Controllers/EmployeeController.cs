using EmployeeManager.Application.Dtos.EmployeeDtos;
using EmployeeManager.Application.Services;
using EmployeeManager.Application.Services.Implementation;
using EmployeeManager.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.API.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET /api/employees/GetPage?page=1&pageSize=10&departmentId=2&searchTerm=ahmed
    [HttpGet("GetPage")]
    public async Task<IActionResult> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? departmentId = null,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _employeeService.GetPageAsync(
            page, pageSize, departmentId, searchTerm, cancellationToken);

        return Ok(result);
    }

    // GET /api/employees/GetById/5
    [HttpGet("GetById/{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeService.GetByIdAsync(id, cancellationToken);

        if (employee is null)
            return NotFound();

        return Ok(employee);
    }

    // POST /api/employees/Create
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
  

    }

    // PUT /api/employees/Update/5
    [HttpPut("Update/{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeService.UpdateAsync(id, dto, cancellationToken);

        if (employee is null)
            return NotFound();

        return Ok(employee);
    }

    // DELETE /api/employees/Delete/5
    [HttpDelete("Delete/{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        await _employeeService.DeleteAsync(id, cancellationToken);
        return NoContent();


    }
}