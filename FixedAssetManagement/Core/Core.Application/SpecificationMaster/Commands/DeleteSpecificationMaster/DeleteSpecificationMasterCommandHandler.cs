using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ISpecificationMaster;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.SpecificationMaster.Commands.DeleteSpecificationMaster
{
    public class DeleteSpecificationMasterCommandHandler : IRequestHandler<DeleteSpecificationMasterCommand, ApiResponseDTO<SpecificationMasterDTO>>
    {
        private readonly ISpecificationMasterCommandRepository _specificationMasterRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly ISpecificationMasterQueryRepository _specificationMasterQueryRepository;
        
        public DeleteSpecificationMasterCommandHandler(ISpecificationMasterCommandRepository specificationMasterRepository, IMapper mapper,  IMediator mediator,ISpecificationMasterQueryRepository specificationMasterQueryRepository)
        {
            _specificationMasterRepository = specificationMasterRepository;
             _mapper = mapper;        
            _mediator = mediator;
            _specificationMasterQueryRepository=specificationMasterQueryRepository;
        }

        public async Task<ApiResponseDTO<SpecificationMasterDTO>> Handle(DeleteSpecificationMasterCommand request, CancellationToken cancellationToken)
        {             
            var specificationMaster = await _specificationMasterQueryRepository.GetByIdAsync(request.Id);
            if (specificationMaster is null )
            {
                return new ApiResponseDTO<SpecificationMasterDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid SpecificationMasterID."
                };
            }
            var specificationMasterDelete = _mapper.Map<SpecificationMasters>(request);      
            var updateResult = await _specificationMasterRepository.DeleteAsync(request.Id, specificationMasterDelete);
            if (updateResult > 0)
            {
                var specificationMasterDto = _mapper.Map<SpecificationMasterDTO>(specificationMasterDelete);  
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: specificationMasterDelete.AssetGroupId.ToString(),
                    actionName: specificationMasterDelete.SpecificationName ?? string.Empty,
                    details: $"Specification Master '{specificationMasterDto.SpecificationName}' was created",
                    module:"Specification Master"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);                 
                return new ApiResponseDTO<SpecificationMasterDTO>
                {
                    IsSuccess = true,
                    Message = "Specification Master deleted successfully.",
                    Data = specificationMasterDto
                };
            }

            return new ApiResponseDTO<SpecificationMasterDTO>
            {
                IsSuccess = false,
                Message = "Specification Master deletion failed."                             
            };           
        }
    }
}