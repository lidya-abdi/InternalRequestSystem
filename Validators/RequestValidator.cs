using FluentValidation;
using InternalRequestSystem.Models;

namespace InternalRequestSystem.Validators
{
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.RequestType)
                .NotEmpty().WithMessage("Request type is required.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Please select a department.");
        }
    }
}