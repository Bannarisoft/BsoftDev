using Core.Application.PartyGroup.Command.CreatePartyGroup;
using Core.Application.PartyGroup.Command.DeletePartyGroup;
using Core.Application.PartyGroup.Command.UpdatePartyGroup;
using FluentValidation;
using PartyManagement.API.Validation.PartyGroup;

namespace PartyManagement.API.Validation.Common
{
    public class ValidationService
    {
        public void AddValidationServices(IServiceCollection services)
        {
            services.AddScoped<MaxLengthProvider>();
            services.AddScoped<IValidator<CreatePartyGroupCommand>, CreatePartyGroupCommandValidator>();
            services.AddScoped<IValidator<DeletePartyGroupCommand>, DeletePartyGroupCommandValidator>();
            services.AddScoped<IValidator<UpdatePartyGroupCommand>, UpdatePartyGroupCommandValidator>();
        }
    }
}