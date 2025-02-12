using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetCategories.Queries.GetAssetCategories;
using Core.Application.AssetGroup.Queries.GetAssetGroup;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetCategories;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetCategories.Queries.GetAssetCategoriesAutoComplete
{
    public class GetAssetCategoriesAutoCompleteQueryHandler :  IRequestHandler<GetAssetCategoriesAutoCompleteQuery,ApiResponseDTO<List<AssetCategoriesAutoCompleteDto>>>
    {
        private readonly IAssetCategoriesQueryRepository _iAssetCategoriesQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetAssetCategoriesAutoCompleteQueryHandler(IAssetCategoriesQueryRepository iAssetCategoriesQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iAssetCategoriesQueryRepository = iAssetCategoriesQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<AssetCategoriesAutoCompleteDto>>> Handle(GetAssetCategoriesAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _iAssetCategoriesQueryRepository.GetAssetCategories(request.SearchPattern);
            var assetcategories  = _mapper.Map<List<AssetCategoriesAutoCompleteDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"AssetCategories details was fetched.",
                    module:"AssetCategories"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<AssetCategoriesAutoCompleteDto>> { IsSuccess = true, Message = "Success", Data = assetcategories };
        }
    }
}