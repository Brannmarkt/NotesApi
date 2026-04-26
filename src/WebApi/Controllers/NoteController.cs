using Application.Common;
using Application.Helpers;
using Application.Interfaces;
using Application.Notes.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ControllerBase
{
    private readonly INoteService _noteService;

    public NoteController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll([FromQuery] NotesQueryOptions options)
    {
        var result = await _noteService.GetAllAsync(options);

        return result.Status switch
        {
            ResultStatus.Success => Ok(result.Data),
            ResultStatus.InvalidData => BadRequest(result.Message),
            ResultStatus.NotFound => NotFound("No notes were found matching your criteria"),
            _ => StatusCode(500, "An unexpected error occurred while fetching notes")
        };
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _noteService.GetByIdAsync(id);

        return result.Status switch
        {
            ResultStatus.NotFound => NotFound($"Note with ID {id} not found"),
            ResultStatus.Success => Ok(result.Data),
            _ => StatusCode(500, "Unexpected error")
        };
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNoteDto dto)
    {
        var result = await _noteService.CreateAsync(dto);
        return result.Status switch
        {
            ResultStatus.InvalidData => BadRequest(result.Message),
            ResultStatus.Success => CreatedAtAction(nameof(Get), new { id = result.Data!.Id }, result.Data),
            _ => StatusCode(500, "Unexpected error")
        };
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateNoteDto dto)
    {
        var result = await _noteService.UpdateAsync(id, dto);
        return result.Status switch
        {
            ResultStatus.NotFound => NotFound(),
            ResultStatus.Success => NoContent(),
            _ => StatusCode(500)
        };
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _noteService.DeleteAsync(id);

        return result.Status switch
        {
            ResultStatus.NotFound => NotFound($"Note with ID {id} not found"),
            ResultStatus.Success => NoContent(), 
            _ => StatusCode(500, "An unexpected error occurred")
        };
    }
}
