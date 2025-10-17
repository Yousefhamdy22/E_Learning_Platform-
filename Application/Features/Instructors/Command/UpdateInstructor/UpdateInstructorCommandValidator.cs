﻿using FluentValidation;

namespace Application.Features.Instructors.Command.UpdateInstructor
{
   

    public class UpdateInstructorCommandValidator : AbstractValidator<UpdateInstructorCommand>
    {
        public UpdateInstructorCommandValidator()
        {
          
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address.");
        }
    }

}
