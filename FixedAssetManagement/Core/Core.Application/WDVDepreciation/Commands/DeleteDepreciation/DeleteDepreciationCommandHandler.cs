using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationDetail;
using Core.Application.Common.Interfaces.IWdvDepreciation;
using Core.Application.WDVDepreciation.Queries.CalculateDepreciation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WDVDepreciation.Commands.DeleteDepreciation
{
    public class DeleteDepreciationCommandHandler : IRequestHandler<DeleteDepreciationCommand, ApiResponseDTO<CalculationDepreciationDto>>
    {
        private readonly IWdvDepreciationCommandRepository _WdvCommandRepository;
        private readonly IWdvDepreciationQueryRepository _WdvQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public DeleteDepreciationCommandHandler(IWdvDepreciationCommandRepository WdvCommandRepository ,IWdvDepreciationQueryRepository WdvQueryRepository, IMapper mapper, IMediator mediator)
        {
            _WdvCommandRepository = WdvCommandRepository;
            _WdvQueryRepository = WdvQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<CalculationDepreciationDto>> Handle(DeleteDepreciationCommand request, CancellationToken cancellationToken)
        {
            var depreciationLocked = await _WdvQueryRepository.ExistDataLockedAsync(request.FinYearId);
            if (depreciationLocked==true)
            {
                return new ApiResponseDTO<CalculationDepreciationDto>
                {
                    IsSuccess = false,
                    Message = "Already depreciation details Locked."
                };
            }
            var depreciationGroups = await _WdvQueryRepository.ExistDataAsync( request.FinYearId);
            if (depreciationGroups==false)
            {
                return new ApiResponseDTO<CalculationDepreciationDto>
                {
                    IsSuccess = false,
                    Message = "No details found for this period"
                };
            }

            var depreciationDelete = _mapper.Map<CalculationDepreciationDto>(request);      
            var updateResult = await _WdvCommandRepository.DeleteAsync( request.FinYearId);
            if (updateResult > 0)
            {
                var depreciationGroupDto = _mapper.Map<CalculationDepreciationDto>(depreciationDelete);  
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: depreciationDelete.FinYear.ToString() ??string.Empty,
                    actionName: "Delete",
                    details: $"WDV Depreciation Details was Deleted. ",
                    module:"WDV Depreciation Deletion"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);                 
                return new ApiResponseDTO<CalculationDepreciationDto>
                {
                    IsSuccess = true,
                    Message = "Depreciation deleted successfully.",
                    Data = depreciationGroupDto
                };
            }
            return new ApiResponseDTO<CalculationDepreciationDto>
            {
                IsSuccess = false,
                Message = "Depreciation deletion failed."                             
            };           
        }
    }
  
}