using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.Commands.CreateNote;
public class CreateNoteCommand : IRequest<int>
{
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
