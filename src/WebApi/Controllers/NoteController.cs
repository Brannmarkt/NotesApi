using Application.Notes.Commands.CreateNote;
using Application.Notes.Commands.DeleteNote;
using Application.Notes.Commands.UpdateNote;
using Application.Notes.Queries.GetNoteById;
using Application.Notes.Queries.GetNotes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoteController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetNotesListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetNoteByIdQuery(id));
        return Ok(result); // NotFoundException обробиться автоматично в ExceptionHandler
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNoteCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNoteCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteNoteCommand(id));
        return NoContent();
    }
}
