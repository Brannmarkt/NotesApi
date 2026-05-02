using Application.Common.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.Commands.UpdateNote;
public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNoteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо нотатку з репозиторію
        var note = await _unitOfWork.Notes.GetByIdAsync(request.Id, cancellationToken);

        if (note == null)
        {
            throw new NotFoundException(nameof(NoteEntity), request.Id);
        }

        // 2. Оновлюємо поля
        note.Title = request.Title;
        note.Text = request.Text;
        note.LastEditDate = DateTime.UtcNow;

        // 3. Викликаємо метод Update (у нашому GenericRepository він просто міняє статус Entity на Modified)
        _unitOfWork.Notes.Update(note);

        // 4. Зберігаємо зміни
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
