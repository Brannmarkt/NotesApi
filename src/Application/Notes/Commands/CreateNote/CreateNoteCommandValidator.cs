using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.Commands.CreateNote;
public class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Заголовок є обов'язковим.")
            .MaximumLength(200).WithMessage("Заголовок не може перевищувати 200 символів.");

        RuleFor(v => v.Text)
            .NotEmpty().WithMessage("Вміст нотатки не може бути порожнім.");
    }
}
