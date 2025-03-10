using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransfer;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransfer.Queries.GetAssetTransfered
{
    public class AssetTransferQueryHandler : IRequestHandler<AssetTransferQuery,  ApiResponseDTO<List<AssetTransferDto>>>
    {
         private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;
        private readonly IMapper _mapper;        
        private readonly IMediator _mediator; 
        public AssetTransferQueryHandler( IAssetTransferQueryRepository assetTransferQueryRepository, IMapper mapper, IMediator mediator)
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
             _mapper = mapper;
            _mediator = mediator;
        }
         public  async Task<ApiResponseDTO<List<AssetTransferDto>>> Handle(AssetTransferQuery request, CancellationToken cancellationToken)        
        {
           // var (assetInsurance, totalCount) = await _assetInsuranceQueryRepository.GetAllAssetInsuranceAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var (assetInsurance, totalCount)  = await _assetTransferQueryRepository.GetAllAsync(request.PageNumber, request.PageSize, request.SearchTerm);
          //  var totalCount = assetInsurance.Count;
            var assetinsuranceList = _mapper.Map<List<AssetTransferDto>>(assetInsurance);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"Asset Insurance details was fetched.",
                module:"Asset Insurance"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<AssetTransferDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetinsuranceList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize                
            };          
        }

      
        
    }
}