using Application.Mappings;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.DTOs;
public class CreateNoteDto : IMapFrom<NoteEntity>
{
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
