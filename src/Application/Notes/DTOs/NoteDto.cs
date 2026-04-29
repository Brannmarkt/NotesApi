using Application.Common.Mappings;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.DTOs;

public class NoteDto : IMapFrom<NoteEntity>
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    // Якщо назви полів dto та entity різні, можна додати:
    // public void Mapping(Profile profile) {
    //    profile.CreateMap<NoteEntity, NoteDto>()
    //           .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Header));
    // }
}
