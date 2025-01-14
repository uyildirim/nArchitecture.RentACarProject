﻿using FluentValidation;

namespace Application.Features.FindeksCreditRates.Commands.UpdateByUserIdFromService
{
    public class UpdateByUserIdFindeksCreditRateFromServiceCommandValidator : AbstractValidator<UpdateByUserIdFindeksCreditRateFromServiceCommand>
    {
        public UpdateByUserIdFindeksCreditRateFromServiceCommandValidator()
        {
            RuleFor(c => c.UserId).GreaterThan(0);
            RuleFor(c => c.IdentityNumber).NotEmpty().MinimumLength(2);
        }
    }
}
