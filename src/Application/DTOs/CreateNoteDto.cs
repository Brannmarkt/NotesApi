using Application.Helpers.Mappers;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;
public class CreateNoteDto : IMapFrom<NoteEntity>
{
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
