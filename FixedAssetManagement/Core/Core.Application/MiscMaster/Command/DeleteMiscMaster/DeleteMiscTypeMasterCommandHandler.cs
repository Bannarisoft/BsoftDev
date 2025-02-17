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

namespace Core.Application.MiscMaster.Command.DeleteMiscMaster
{
    public class DeleteMiscTypeMasterCommandHandler  : IRequestHandler<DeleteMiscMasterCommand, ApiResponseDTO<GetMiscMasterDto>>
    {

         private readonly IMiscMasterCommandRepository _miscMasterCommandRepository;
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;



        public DeleteMiscTypeMasterCommandHandler(IMiscMasterCommandRepository miscMasterCommandRepository, IMapper imapper, IMediator mediator)
        {
            _miscMasterCommandRepository = miscMasterCommandRepository;
            _imapper = imapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<GetMiscMasterDto>> Handle(DeleteMiscMasterCommand request, CancellationToken cancellationToken)
        {
            // Map the request to the entity
            var miscMaster = _imapper.Map<Core.Domain.Entities.MiscMaster>(request);

            // Perform the delete operation
            var isDeleted = await _miscMasterCommandRepository.DeleteAsync(request.Id, miscMaster);

            // Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: miscMaster.Id.ToString(),
                actionName: miscMaster.IsDeleted.ToString(),
                details: $"MiscMaster with ID {miscMaster.Id} was deleted.",
                module: "MiscMaster"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            // Return the response based on the result
            if (isDeleted)
            {
                return new ApiResponseDTO<GetMiscMasterDto>
                {
                    IsSuccess = true,
                    Message = "MiscMaster deleted successfully."
                };
            }

            return new ApiResponseDTO<GetMiscMasterDto>
            {
                IsSuccess = false,
                Message = "MiscMaster not deleted."
            };
        }
        
    }
}