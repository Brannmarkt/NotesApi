using Application.Common.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.Commands.DeleteNote;
public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNoteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _unitOfWork.Notes.GetByIdAsync(request.Id, cancellationToken);

        if (note == null)
        {
            throw new NotFoundException(nameof(NoteEntity), request.Id);
        }

        _unitOfWork.Notes.Delete(note);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
