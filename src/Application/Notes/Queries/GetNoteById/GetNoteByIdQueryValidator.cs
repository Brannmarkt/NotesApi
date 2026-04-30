using FluentValidation;

namespace Application.Notes.Queries.GetNoteById;
public class GetNoteByIdQueryValidator : AbstractValidator<GetNoteByIdQuery>
{
    public GetNoteByIdQueryValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty()
            .GreaterThan(0).WithMessage("ID нотатки має бути більше нуля.");
    }
}
