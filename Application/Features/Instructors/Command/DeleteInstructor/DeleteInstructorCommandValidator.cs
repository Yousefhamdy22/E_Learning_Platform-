using FluentValidation;
namespace Application.Features.Instructors.Command.DeleteInstructor
{
    

    public class DeleteInstructorCommandValidator : AbstractValidator<DeleteInstructorCommand>
    {
        public DeleteInstructorCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Instructor Id is required.");
        }
    }

}
