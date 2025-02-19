
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ISpecificationMaster;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.SpecificationMaster.Commands.CreateSpecificationMaster
{
    public class CreateSpecificationMasterCommandHandler : IRequestHandler<CreateSpecificationMasterCommand, ApiResponseDTO<SpecificationMasterDTO>>
    {        
        private readonly IMapper _mapper;
        private readonly ISpecificationMasterCommandRepository _specificationMasterRepository;
        private readonly IMediator _mediator;

        public CreateSpecificationMasterCommandHandler(IMapper mapper, ISpecificationMasterCommandRepository specificationMasterRepository, IMediator mediator)
        {
            _mapper = mapper;
            _specificationMasterRepository = specificationMasterRepository;
            _mediator = mediator;    
        } 

       public async Task<ApiResponseDTO<SpecificationMasterDTO>> Handle(CreateSpecificationMasterCommand request, CancellationToken cancellationToken)
        {
            var specificationMasterExists = await _specificationMasterRepository.ExistsByAssetGroupIdAsync(request.AssetGroupId,request.SpecificationName);
            if (specificationMasterExists)
            {
                return new ApiResponseDTO<SpecificationMasterDTO> {
                    IsSuccess = false, 
                    Message = "SpecificationMaster already exists."
                };                 
            }
       
            var specificationMasterEntity = _mapper.Map<SpecificationMasters>(request);            
            var result = await _specificationMasterRepository.CreateAsync(specificationMasterEntity);
            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: specificationMasterEntity.AssetGroupId.ToString(),
                actionName: specificationMasterEntity.SpecificationName ?? string.Empty,
                details: $"SpecificationMaster '{specificationMasterEntity.SpecificationName}' was created.",
                module:"SpecificationMaster"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var specificationMasterDto = _mapper.Map<SpecificationMasterDTO>(result);
            if (specificationMasterDto.Id > 0)
            {
                return new ApiResponseDTO<SpecificationMasterDTO>{
                    IsSuccess = true, 
                    Message = "SpecificationMaster created successfully.",
                    Data = specificationMasterDto
                };
            }
            return  new ApiResponseDTO<SpecificationMasterDTO>{
                IsSuccess = false, 
                Message = "SpecificationMaster not created."
            };      
        }
    }
}