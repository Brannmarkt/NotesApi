using Application.Common.Pagination;
using Application.Notes.DTOs;
using MediatR;

namespace Application.Notes.Queries.GetNotes;
public class GetNotesListQuery : IRequest<PaginatedList<NoteDto>>
{
    public string? SearchTerm { get; set; } // Фільтрація по заголовку
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
