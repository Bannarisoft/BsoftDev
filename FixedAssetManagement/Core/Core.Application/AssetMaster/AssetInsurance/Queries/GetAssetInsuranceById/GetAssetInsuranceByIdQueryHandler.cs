using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsurance;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetInsurance;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsuranceById
{
    public class GetAssetInsuranceByIdQueryHandler  : IRequestHandler<GetAssetInsuranceByIdQuery, ApiResponseDTO<GetAssetInsuranceDto>>
    {   

         private readonly IAssetInsuranceQueryRepository  _assetInsuranceQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        

        public GetAssetInsuranceByIdQueryHandler(IAssetInsuranceQueryRepository  assetInsuranceQueryRepository, IMapper mapper, IMediator mediator)
        {
            _assetInsuranceQueryRepository = assetInsuranceQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
       
       public async Task<ApiResponseDTO<GetAssetInsuranceDto>> Handle(GetAssetInsuranceByIdQuery request, CancellationToken cancellationToken)
        {
            var assetInsurance = await _assetInsuranceQueryRepository.GetByAssetIdAsync(request.Id);
             var assetinsuranceDto = _mapper.Map<GetAssetInsuranceDto>(assetInsurance);
             if (assetInsurance is null)
            {                
                return new ApiResponseDTO<GetAssetInsuranceDto>
                {
                    IsSuccess = false,
                    Message = "AssetLocation with ID {request.Id} not found."
                };   
            }      

              //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: assetinsuranceDto.AssetId == null ? "" : assetinsuranceDto.AssetId.ToString(),        
                actionName: assetinsuranceDto.PolicyNo == null ? "" : assetinsuranceDto.PolicyNo.ToString(),                
                details: $"Asset '{assetinsuranceDto.AssetId}' was created. Code: {assetinsuranceDto.Id}",
                module:"AssetMasterGeneral"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<GetAssetInsuranceDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetinsuranceDto
            };       

        }
    }
}