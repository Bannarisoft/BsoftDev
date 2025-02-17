using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetSubCategories.Queries.GetAssetSubCategories;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetSubCategories;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetSubCategories.Queries.GetAssetSubCategoriesAutoComplete
{
    public class GetAssetSubCategoriesAutoCompleteQueryHandler :  IRequestHandler<GetAssetSubCategoriesAutoCompleteQuery,ApiResponseDTO<List<AssetSubCategoriesAutoCompleteDto>>>
    {
        
        private readonly IAssetSubCategoriesQueryRepository _iAssetSubCategoriesQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetAssetSubCategoriesAutoCompleteQueryHandler(IAssetSubCategoriesQueryRepository iAssetSubCategoriesQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iAssetSubCategoriesQueryRepository = iAssetSubCategoriesQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }
        
        public async Task<ApiResponseDTO<List<AssetSubCategoriesAutoCompleteDto>>> Handle(GetAssetSubCategoriesAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _iAssetSubCategoriesQueryRepository.GetAssetSubCategories(request.SearchPattern);
            var assetsubcategories  = _mapper.Map<List<AssetSubCategoriesAutoCompleteDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"AssetSubCategories details was fetched.",
                    module:"AssetSubCategories"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<AssetSubCategoriesAutoCompleteDto>> { IsSuccess = true, Message = "Success", Data = assetsubcategories };
        }

    }
}