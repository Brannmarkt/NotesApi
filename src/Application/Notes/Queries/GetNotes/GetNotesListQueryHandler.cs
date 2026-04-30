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
        Expression<Func<NoteEntity, bool>>? filter = string.IsNullOrWhiteSpace(request.SearchTerm)
            ? null
            : n => n.Title.ToLower().Contains(request.SearchTerm.ToLower())
                || n.Text.ToLower().Contains(request.SearchTerm.ToLower());

        var (items, totalCount) = await _unitOfWork.Notes.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter, // Передаємо сформований фільтр
            cancellationToken);

        var dtos = _mapper.Map<IReadOnlyCollection<NoteDto>>(items);

        return new PaginatedList<NoteDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
