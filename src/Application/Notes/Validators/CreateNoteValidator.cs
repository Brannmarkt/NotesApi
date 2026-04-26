using Application.Notes.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notes.Validators;
public class CreateNoteValidator : AbstractValidator<CreateNoteDto>
{
    public CreateNoteValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Text cannot be empty.")
            .MinimumLength(5).WithMessage("Text must be at least 5 characters long.");
    }
}
