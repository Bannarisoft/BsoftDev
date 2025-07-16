using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssertByCategory
{
    public class GetAssetsByCategoryQueryHandler  : IRequestHandler<GetAssetsByCategoryQuery,  ApiResponseDTO<List<GetAssetMasterDto>>>
    {


        private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;
        private readonly IMapper _mapper;
        public readonly IMediator _mediator;
        public GetAssetsByCategoryQueryHandler( IAssetTransferQueryRepository assetTransferQueryRepository, IMapper mapper, IMediator mediator )
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
            _mapper = mapper;
            _mediator=mediator;
        }
        public async Task<ApiResponseDTO<List<GetAssetMasterDto>>> Handle(GetAssetsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var assets = await _assetTransferQueryRepository.GetAssetsByCategoryAsync(request.AssetCategoryId , request.AssetDepartmentId);

            //return _mapper.Map<List<GetAssetMasterDto>>(assets);
            var AssetList = _mapper.Map<List<GetAssetMasterDto>>(assets);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"Asset Category    details was fetched.",
                module:"Asset Category"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<GetAssetMasterDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = AssetList
                         
            };           

          
        }


        
    }
}