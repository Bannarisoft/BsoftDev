using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ISpecificationMaster;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.SpecificationMaster.Queries.GetSpecificationMasterAutoComplete
{
    public class GetSpecificationMasterAutoCompleteQueryHandler : IRequestHandler<GetSpecificationMasterAutoCompleteQuery, ApiResponseDTO<List<SpecificationMasterAutoCompleteDTO>>>
    {
        private readonly ISpecificationMasterQueryRepository _specificationMasterRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetSpecificationMasterAutoCompleteQueryHandler(ISpecificationMasterQueryRepository specificationMasterRepository,  IMapper mapper, IMediator mediator)
        {
            _specificationMasterRepository =specificationMasterRepository;
            _mapper =mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<SpecificationMasterAutoCompleteDTO>>> Handle(GetSpecificationMasterAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _specificationMasterRepository.GetBySpecificationNameAsync(request.SearchPattern ?? string.Empty);
            if (result is null || result.Count is 0)
            {
                return new ApiResponseDTO<List<SpecificationMasterAutoCompleteDTO>>
                {
                    IsSuccess = false,
                    Message = "No SpecificationMaster found matching the search pattern."
                };
            }
            var specificationMasterDto = _mapper.Map<List<SpecificationMasterAutoCompleteDTO>>(result);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAutoComplete",
                actionCode:"",        
                actionName: request.SearchPattern ?? string.Empty,                
                details: $"SpecificationMaster '{request.SearchPattern}' was searched",
                module:"SpecificationMaster"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<SpecificationMasterAutoCompleteDTO>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = specificationMasterDto
            };          
        }      
    }
}