using FluentValidation;
using InternalRequestSystem.Models;

namespace InternalRequestSystem.Validators
{
    public class ApprovalValidator : AbstractValidator<Approval>
    {
        public ApprovalValidator()
        {
            RuleFor(x => x.Decision)
                .NotEmpty().WithMessage("Decision is required.");
        }
    }
}