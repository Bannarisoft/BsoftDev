using Core.Application.MiscMaster.Command.CreateMiscMaster;
using Core.Application.MiscMaster.Command.DeleteMiscMaster;
using Core.Application.MiscMaster.Command.UpdateMiscMaster;
using Core.Application.MiscTypeMaster.Command.CreateMiscTypeMaster;
using Core.Application.MiscTypeMaster.Command.DeleteMiscTypeMaster;
using Core.Application.MiscTypeMaster.Command.UpdateMiscTypeMaster;
using Core.Application.PartyGroup.Command.CreatePartyGroup;
using Core.Application.PartyGroup.Command.DeletePartyGroup;
using Core.Application.PartyGroup.Command.UpdatePartyGroup;
using FluentValidation;
using PartyManagement.API.Validation.MiscMaster;
using PartyManagement.API.Validation.MiscTypeMaster;
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
            services.AddScoped<IValidator<CreateMiscTypeMasterCommand>, CreateMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteMiscTypeMasterCommand>, DeleteMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateMiscTypeMasterCommand>, UpdateMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<CreateMiscMasterCommand>, CreateMiscMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteMiscMasterCommand>, DeleteMiscMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateMiscMasterCommand>, UpdateMiscMasterCommandValidator>();
        }
    }
}