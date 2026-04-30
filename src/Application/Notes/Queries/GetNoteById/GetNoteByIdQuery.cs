using Application.Notes.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.Queries.GetNoteById;
public record GetNoteByIdQuery(int Id) : IRequest<NoteDto?>;