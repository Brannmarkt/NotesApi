using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.Commands.UpdateNote;
public class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Заголовок не може бути порожнім.")
            .MaximumLength(200);

        RuleFor(v => v.Text)
            .NotEmpty();
    }
}
