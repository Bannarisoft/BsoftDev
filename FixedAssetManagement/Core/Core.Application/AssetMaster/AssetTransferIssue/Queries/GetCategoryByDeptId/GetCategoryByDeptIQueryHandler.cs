using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetCategoryByDeptId
{
    public class GetCategoryByDeptIQueryHandler  : IRequestHandler<GetCategoryByDeptIQuery,  ApiResponseDTO<List<GetCategoryByDeptIdDto>>>
    {
        private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;

           private readonly IMapper _mapper;        
        private readonly IMediator _mediator; 

           public GetCategoryByDeptIQueryHandler( IAssetTransferQueryRepository assetTransferQueryRepository, IMapper mapper, IMediator mediator)
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
         
        }

          public async Task<ApiResponseDTO<List<GetCategoryByDeptIdDto>>> Handle(GetCategoryByDeptIQuery request, CancellationToken cancellationToken)
        {
            var CategoryList = await _assetTransferQueryRepository.GetCategoriesByDepartmentAsync(request.DepartmentId);

             
            var AssetTransferList = _mapper.Map<List<GetCategoryByDeptIdDto>>(CategoryList);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"Asset Category    details was fetched.",
                module:"Asset Category"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<GetCategoryByDeptIdDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = AssetTransferList
                         
            };                 
        }
  
    }
}