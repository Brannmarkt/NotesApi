using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.Commands.UpdateNote;
public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNoteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо нотатку з репозиторію
        var note = await _unitOfWork.Notes.GetByIdAsync(request.Id, cancellationToken);

        if (note == null)
        {
            return false; // Або кинути NotFoundException
        }

        // 2. Оновлюємо поля
        note.Title = request.Title;
        note.Text = request.Text;
        note.LastEditDate = DateTime.UtcNow;

        // 3. Викликаємо метод Update (у нашому GenericRepository він просто міняє статус Entity на Modified)
        _unitOfWork.Notes.Update(note);

        // 4. Зберігаємо зміни
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
