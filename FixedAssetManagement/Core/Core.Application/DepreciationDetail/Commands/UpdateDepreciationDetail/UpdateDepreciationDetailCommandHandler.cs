using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationDetail;
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.DepreciationDetail.Commands.UpdateDepreciationDetail
{
    public class UpdateDepreciationDetailCommandHandler : IRequestHandler<UpdateDepreciationDetailCommand, ApiResponseDTO<DepreciationDto>>
    {
        private readonly IDepreciationDetailCommandRepository _depreciationRepository;
        private readonly IDepreciationDetailQueryRepository _depreciationQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
                
        public UpdateDepreciationDetailCommandHandler(IDepreciationDetailCommandRepository depreciationRepository, IMapper mapper,  IMediator mediator,IDepreciationDetailQueryRepository depreciationQueryRepository)
        {
            _depreciationRepository = depreciationRepository;
            _mapper = mapper;        
            _mediator = mediator;            
            _depreciationQueryRepository=depreciationQueryRepository;
        }

        public async Task<ApiResponseDTO<DepreciationDto>> Handle(UpdateDepreciationDetailCommand request, CancellationToken cancellationToken)
        {             
            
            var depreciationLocked = await _depreciationQueryRepository.ExistDataLockedAsync(request.companyId,request.unitId, request.finYear??string.Empty, request.depreciationType,request.depreciationPeriod);
            if (depreciationLocked==true)
            {
                return new ApiResponseDTO<DepreciationDto>
                {
                    IsSuccess = false,
                    Message = "Already depreciation details Locked."
                };
            }
            var depreciationGroups = await _depreciationQueryRepository.ExistDataAsync(request.companyId,request.unitId, request.finYear??string.Empty, request.depreciationType,request.depreciationPeriod);
            if (depreciationGroups==false)
            {
                return new ApiResponseDTO<DepreciationDto>
                {
                    IsSuccess = false,
                    Message = "No details found for this period"
                };
            }
            
            var depreciationDelete = _mapper.Map<DepreciationDetails>(request);      
            var updateResult = await _depreciationRepository.UpdateAsync(request.companyId, request.unitId, request.finYear??string.Empty, request.depreciationType,request.depreciationPeriod);
            if (updateResult > 0)
            {
                var depreciationGroupDto = _mapper.Map<DepreciationDto>(depreciationDelete);  
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: depreciationDelete.Finyear ?? string.Empty,
                    actionName: "Update",
                    details: $"Depreciation Details '{depreciationGroupDto.Company}' was Deleted. Code: {depreciationGroupDto.Unit}",
                    module:"DepreciationDetail"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);                 
                return new ApiResponseDTO<DepreciationDto>
                {
                    IsSuccess = true,
                    Message = "Depreciation Locked successfully.",
                    Data = depreciationGroupDto
                };
            }
            return new ApiResponseDTO<DepreciationDto>
            {
                IsSuccess = false,
                Message = "Depreciation Locked failed."                             
            };           
        }
    }
}