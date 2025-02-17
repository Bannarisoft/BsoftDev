using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetCategories.Queries.GetAssetCategories;
using Core.Application.AssetSubCategories.Queries.GetAssetSubCategories;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetCategories;
using Core.Application.Common.Interfaces.IAssetSubCategories;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetSubCategories.Queries.GetAssetSubCategoriesById
{
    public class GetAssetSubCategoriesByIdQueryHandler: IRequestHandler<GetAssetSubCategoriesByIdQuery,ApiResponseDTO<AssetSubCategoriesDto>>
    {
        private readonly IAssetSubCategoriesQueryRepository _iAssetSubCategoriesQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetAssetSubCategoriesByIdQueryHandler(IAssetSubCategoriesQueryRepository iAssetSubCategoriesQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iAssetSubCategoriesQueryRepository = iAssetSubCategoriesQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }
         public async Task<ApiResponseDTO<AssetSubCategoriesDto>> Handle(GetAssetSubCategoriesByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _iAssetSubCategoriesQueryRepository.GetByIdAsync(request.Id);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<AssetSubCategoriesDto> { IsSuccess = false, Message =$"AssetSubCategories ID {request.Id} not found." };
            }
            // Map a single entity
            var assetSubCategories = _mapper.Map<AssetSubCategoriesDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "",        
                    actionName: "",
                    details: $"AssetSubCategories details {assetSubCategories.Id} was fetched.",
                    module:"AssetSubCategories"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<AssetSubCategoriesDto> { IsSuccess = true, Message = "Success", Data = assetSubCategories };
        }

    }
}