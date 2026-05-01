using Application.Common.Pagination;
using Application.Interfaces;
using Application.Notes.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Notes.Queries.GetNotes;
public class GetNotesListQueryHandler : IRequestHandler<GetNotesListQuery, PaginatedList<NoteDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetNotesListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedList<NoteDto>> Handle(GetNotesListQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Notes.GetAll();

        // 1. Фільтрація
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(n => n.Title.Contains(request.SearchTerm) ||
                                    n.Text.Contains(request.SearchTerm));
        }

        // 2. Сортування
        query = query.OrderByDescending(n => n.CreationDate);

        // 3. Отримання даних з репозиторію (повертає Items та TotalCount)
        var (items, totalCount) = await _unitOfWork.Notes.GetPagedAsync(
            query,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        // 4. Мапимо тільки список сутностей у список DTO
        var dtos = _mapper.Map<IReadOnlyCollection<NoteDto>>(items);

        // 5. Вручну створюємо PaginatedList (AutoMapper тут не потрібен)
        return new PaginatedList<NoteDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize);
    }
}
