using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.Commands.CreateNote;
public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateNoteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new NoteEntity
        {
            Title = request.Title,
            Text = request.Text,
            CreationDate = DateTime.UtcNow
        };

        await _unitOfWork.Notes.AddAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return note.Id;
    }
}
