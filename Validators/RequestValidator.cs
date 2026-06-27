using FluentValidation;
using InternalRequestSystem.Models;

namespace InternalRequestSystem.Validators
{
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("FROM FLUENT VALIDATION")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters."); 

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("FluentValidation: Description is required")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.RequestType)
                .NotEmpty().WithMessage("FluentValidation: Request type is required");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("FluentValidation: Please select a department");
        }
    }
}