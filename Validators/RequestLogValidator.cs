using FluentValidation;
using InternalRequestSystem.Models;

namespace InternalRequestSystem.Validators
{
    public class RequestLogValidator : AbstractValidator<RequestLog>
    {
        public RequestLogValidator()
        {
            RuleFor(x => x.Action)
                .NotEmpty().WithMessage("Action is required.");
        }
    }
}