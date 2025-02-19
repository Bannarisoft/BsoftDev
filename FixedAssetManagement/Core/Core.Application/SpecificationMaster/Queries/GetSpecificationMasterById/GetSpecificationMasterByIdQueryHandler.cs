using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ISpecificationMaster;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.SpecificationMaster.Queries.GetSpecificationMasterById
{
    public class GetSpecificationMasterByIdQueryHandler : IRequestHandler<GetSpecificationMasterByIdQuery, ApiResponseDTO<SpecificationMasterDTO>>
    {
        private readonly ISpecificationMasterQueryRepository _specificationMasterRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetSpecificationMasterByIdQueryHandler(ISpecificationMasterQueryRepository specificationMasterRepository,  IMapper mapper, IMediator mediator)
        {
            _specificationMasterRepository =specificationMasterRepository;
            _mapper =mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<SpecificationMasterDTO>> Handle(GetSpecificationMasterByIdQuery request, CancellationToken cancellationToken)
        {
            var specificationMaster = await _specificationMasterRepository.GetByIdAsync(request.Id);                
            var specificationMasterDto = _mapper.Map<SpecificationMasterDTO>(specificationMaster);
            if (specificationMaster is null)
            {                
                return new ApiResponseDTO<SpecificationMasterDTO>
                {
                    IsSuccess = false,
                    Message = "SpecificationMaster with ID {request.Id} not found."
                };   
            }       
                //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: specificationMasterDto.AssetGroupId.ToString() ?? string.Empty,        
                actionName: specificationMasterDto.SpecificationName ?? string.Empty,                
                details: $"SpecificationMaster '{specificationMasterDto.SpecificationName}' was created",
                module:"SpecificationMaster"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<SpecificationMasterDTO>
            {
                IsSuccess = true,
                Message = "Success",
                Data = specificationMasterDto
            };       
        }
    }
}