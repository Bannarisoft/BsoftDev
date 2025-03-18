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

namespace Core.Application.AssetSubCategories.Queries.GetAssetSubCategoriesByCategoryId
{
    public class GetAssetSubCategoriesByCategoryIdQueryHandler : IRequestHandler<GetAssetSubCategoriesByCategoryIdQuery,ApiResponseDTO<List<AssetSubCategoriesAutoCompleteDto>>>
    {
        
        private readonly IAssetSubCategoriesQueryRepository _iAssetSubCategoriesQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetAssetSubCategoriesByCategoryIdQueryHandler(IAssetSubCategoriesQueryRepository iAssetSubCategoriesQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iAssetSubCategoriesQueryRepository = iAssetSubCategoriesQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<AssetSubCategoriesAutoCompleteDto>>> Handle(GetAssetSubCategoriesByCategoryIdQuery request, CancellationToken cancellationToken)
        {
             var result = await _iAssetSubCategoriesQueryRepository.GetSubcategoriesByAssetCategoryIdAsync(request.AssetCategoriesId);

            // Check if data exists
            if (result is null || !result.Any())
            {
                return new ApiResponseDTO<List<AssetSubCategoriesAutoCompleteDto>>
                {
                    IsSuccess = false,
                    Message = $"No records found for ID {request.AssetCategoriesId}."
                };
            }

            // Map list of results
            var assetSubCategoriesList = _mapper.Map<List<AssetSubCategoriesAutoCompleteDto>>(result);

            // Domain Event Logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "AssetCategoryIdBasedSubCategory",
                actionName: request.AssetCategoriesId.ToString(),
                details: $"Asset SubCategory details for ID {request.AssetCategoriesId} were fetched.",
                module: "AssetCategoryBasedSubCategory"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<List<AssetSubCategoriesAutoCompleteDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetSubCategoriesList
            };
        }
    }
}