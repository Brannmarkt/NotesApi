using Application.Common.Exceptions;
using Application.Interfaces;
using Application.Notes.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.Queries.GetNoteById;
public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, NoteDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetNoteByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    async Task<NoteDto?> IRequestHandler<GetNoteByIdQuery, NoteDto?>.Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо сутність за ID
        var note = await _unitOfWork.Notes.GetByIdAsync(request.Id, cancellationToken);

        // 2. Якщо нотатку не знайдено, повертаємо null
        // (WebApi контролер пізніше перетворить цей null у відповідь 404 Not Found)
        if (note == null)
        {
            throw new NotFoundException(nameof(NoteEntity), request.Id);
            //return null;
        }

        // 3. Мапимо сутність у DTO
        return _mapper.Map<NoteDto>(note);
    }
}
