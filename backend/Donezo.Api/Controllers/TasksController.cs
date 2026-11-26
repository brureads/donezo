using System.Security.Claims;
using Donezo.Application.DTOs.Tasks;
using Donezo.Application.Interfaces;
using Donezo.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = Donezo.Domain.Enums.TaskStatus;

namespace Donezo.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    private Guid UserId =>
        Guid.Parse(User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] TaskStatus? status,
        [FromQuery] TaskPriority? priority,
        [FromQuery] Guid? categoryId)
    {
        if (status.HasValue && !Enum.IsDefined(status.Value))
            return BadRequest(new { message = "Status inválido." });

        if (priority.HasValue && !Enum.IsDefined(priority.Value))
            return BadRequest(new { message = "Prioridade inválida." });

        return Ok(await _service.GetAllAsync(
            UserId, status, priority, categoryId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            return Ok(await _service.GetByIdAsync(UserId, id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskRequest request)
    {
        try
        {
            var task = await _service.CreateAsync(
                UserId, request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = task.Id },
                task);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, TaskRequest request)
    {
        try
        {
            return Ok(await _service.UpdateAsync(
                UserId, id, request));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(UserId, id);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}