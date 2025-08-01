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
using FluentValidation;
using MediatR;

namespace Core.Application.AssetCategories.Queries.GetAssetCategoriesById
{
    public class GetAssetCategoriesByIdQueryHandler : IRequestHandler<GetAssetCategoriesByIdQuery,AssetCategoriesDto>
    {
        private readonly IAssetCategoriesQueryRepository _iAssetCategoriesQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetAssetCategoriesByIdQueryHandler(IAssetCategoriesQueryRepository iAssetCategoriesQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iAssetCategoriesQueryRepository = iAssetCategoriesQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<AssetCategoriesDto> Handle(GetAssetCategoriesByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _iAssetCategoriesQueryRepository.GetByIdAsync(request.Id);
            
            if (result is null)
            {
                throw new ValidationException( $"AssetCategories ID {request.Id} not found.");
                
            }
            
            var assetGroup = _mapper.Map<AssetCategoriesDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "",        
                    actionName: "",
                    details: $"AssetCategories details {assetGroup.Id} was fetched.",
                    module:"AssetCategories"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return  assetGroup;
        }
    }
}