using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetCategoryByCustodian
{
    public class GetCategoryByCustodianQueryHandler : IRequestHandler<GetCategoryByCustodianQuery, ApiResponseDTO<List<GetCategoryByCustodianDto>>>
    {
        private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;

        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetCategoryByCustodianQueryHandler(IAssetTransferQueryRepository assetTransferQueryRepository, IMapper mapper, IMediator mediator)
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
          public async Task<ApiResponseDTO<List<GetCategoryByCustodianDto>>> Handle(GetCategoryByCustodianQuery request, CancellationToken cancellationToken)
        {
            var CategoryList = await _assetTransferQueryRepository.GetCategoryByCustodianAsync(request.CustodianId,request.DepartmentId);

             
            var AssetTransferList = _mapper.Map<List<GetCategoryByCustodianDto>>(CategoryList);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"Asset Category    details was fetched.",
                module:"Asset Category"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<GetCategoryByCustodianDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = AssetTransferList
                         
            };                 
        }
    }
}