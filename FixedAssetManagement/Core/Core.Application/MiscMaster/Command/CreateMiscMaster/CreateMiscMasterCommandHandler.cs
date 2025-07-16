using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MiscMaster.Command.CreateMiscMaster
{
    public class CreateMiscMasterCommandHandler  : IRequestHandler<CreateMiscMasterCommand, ApiResponseDTO<GetMiscMasterDto>>
    {
       

        
        private readonly IMiscMasterCommandRepository _miscMasterCommandRepository;
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;

         public CreateMiscMasterCommandHandler (IMiscMasterCommandRepository miscMasterCommandRepository, IMapper imapper, IMediator mediator, IMiscMasterQueryRepository miscMasterQueryRepository)
        {
            _miscMasterCommandRepository = miscMasterCommandRepository;
            _imapper = imapper;
            _mediator = mediator;
            _miscMasterQueryRepository = miscMasterQueryRepository;
        }
        public  async Task<ApiResponseDTO<GetMiscMasterDto>> Handle(CreateMiscMasterCommand request, CancellationToken cancellationToken)
        {
                // 🔹 Check if a MiscTypeMaster with the same name already exists
            var existingMiscMaster = await _miscMasterQueryRepository.GetByMiscMasterCodeAsync(request.Code,request.MiscTypeId) ;

            if (existingMiscMaster != null)
            {
                return new ApiResponseDTO<GetMiscMasterDto>
                {
                    IsSuccess = false,
                    Message = "Misc  Master already exists",
                    Data = null
                };
            }

            // 🔹 Map request to domain entity
            var miscMaster = _imapper.Map<Core.Domain.Entities.MiscMaster>(request);

            // 🔹 Insert into the database

             var result = await _miscMasterCommandRepository.CreateAsync(miscMaster);
              if (result.Id <= 0)
                {
                return new ApiResponseDTO<GetMiscMasterDto>
                {
                    IsSuccess = false,
                    Message = "Failed to create Misc  Master",
                    Data = null
                };
            }

            // 🔹 Fetch newly created record
            var createdMiscMaster = await _miscMasterQueryRepository.GetByIdAsync(result.Id);
            var mappedResult = _imapper.Map<GetMiscMasterDto>(createdMiscMaster);

            // 🔹 Publish domain event for auditing/logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: miscMaster.Code,
                actionName: miscMaster.Description,
                details: $"Misc  Master '{miscMaster.Code}' was created.",
                module: "MiscMaster"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            // 🔹 Return success response
            return new ApiResponseDTO<GetMiscMasterDto>
            {
                IsSuccess = true,
                Message = "Misc  Master created successfully",
                Data = mappedResult
            };




        }
    }
}