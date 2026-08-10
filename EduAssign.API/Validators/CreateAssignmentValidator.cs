using FluentValidation;
using EduAssign.API.DTOs.Assignments;
using System;

namespace EduAssign.API.Validators
{
    public class CreateAssignmentValidator : AbstractValidator<CreateAssignmentRequest>
    {
        public CreateAssignmentValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.ClassId).NotEmpty().WithMessage("Class selection is required.");
            RuleFor(x => x.SubjectId).NotEmpty().WithMessage("Subject selection is required.");

            RuleFor(x => x.Deadline)
                .GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future.");

            RuleFor(x => x.MaximumMarks)
                .GreaterThan(0).WithMessage("Maximum marks must be greater than 0.");

            RuleFor(x => x.Status)
                .Must(status => status == "Draft" || status == "Published")
                .WithMessage("Status must be either 'Draft' or 'Published'.");
        }
    }
}