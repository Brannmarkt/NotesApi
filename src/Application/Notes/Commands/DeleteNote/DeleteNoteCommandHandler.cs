using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.Commands.DeleteNote;
public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNoteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _unitOfWork.Notes.GetByIdAsync(request.Id, cancellationToken);

        if (note == null)
        {
            return false;
        }

        _unitOfWork.Notes.Delete(note);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
