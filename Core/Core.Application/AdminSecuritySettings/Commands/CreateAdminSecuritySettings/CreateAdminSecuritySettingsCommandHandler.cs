using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;
using MediatR;

namespace Core.Application.AdminSecuritySettings.Commands.CreateAdminSecuritySettings
{
    public class CreateAdminSecuritySettingsCommandHandler  :IRequestHandler<CreateAdminSecuritySettingsCommand, AdminSecuritySettingsDto >
    {
        private readonly IAdminSecuritySettingsCommandRepository _adminSecuritySettingsCommandRepository;
        private readonly IMapper _mapper;

          public CreateAdminSecuritySettingsCommandHandler(IAdminSecuritySettingsCommandRepository adminSecuritySettingsCommandRepository,IMapper mapper)
        {
             _adminSecuritySettingsCommandRepository=adminSecuritySettingsCommandRepository;
            _mapper=mapper;
        }

        public async Task<AdminSecuritySettingsDto> Handle(CreateAdminSecuritySettingsCommand request, CancellationToken cancellationToken)
        {
              var adminSecuritySettings = _mapper.Map<Core.Domain.Entities.AdminSecuritySettings>(request);
          //  departmentEntity.Id = ID; // Assign the new GUID to the user entity

            // Save the user to the repository
            var createdAdminSecuritySettings = await _adminSecuritySettingsCommandRepository.CreateAsync(adminSecuritySettings);
            
            if (createdAdminSecuritySettings == null)
            {
                throw new InvalidOperationException("Failed to create user");
            }

            // Map the created User entity to UserDto
            return _mapper.Map<AdminSecuritySettingsDto>(createdAdminSecuritySettings);
        }




    }


}