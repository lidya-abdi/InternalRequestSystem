using FluentValidation;
using InternalRequestSystem.Models;

namespace InternalRequestSystem.Validators
{
    public class DepartmentValidator : AbstractValidator<Department>
    {
        public DepartmentValidator()
        {
            RuleFor(x => x.DepartmentName)
                .NotEmpty().WithMessage("Department name is required.");
        }
    }
}