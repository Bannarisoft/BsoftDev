using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetCategories;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetCategories.Command.DeleteAssetCategories
{
    public class DeleteAssetCategoriesCommandHandler : IRequestHandler<DeleteAssetCategoriesCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetCategoriesCommandRepository _iAssetCategoryCommandRepository;
        private readonly IAssetCategoriesQueryRepository _iAssetCategoryQueryRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _mediator;

        public DeleteAssetCategoriesCommandHandler(IAssetCategoriesCommandRepository iAssetCategoryCommandRepository,IAssetCategoriesQueryRepository iAssetCategoryQueryRepository,IMapper imapper,IMediator imediator )
        {
            _iAssetCategoryCommandRepository=iAssetCategoryCommandRepository;
            _iAssetCategoryQueryRepository=iAssetCategoryQueryRepository;
            _Imapper=imapper;
            _mediator=imediator;    
        }

        public async Task<ApiResponseDTO<int>> Handle(DeleteAssetCategoriesCommand request, CancellationToken cancellationToken)
        {
            // 🔹 First, check if the ID exists in the database
            var existingAssetCategory = await _iAssetCategoryQueryRepository.GetByIdAsync(request.Id);
            if (existingAssetCategory is null)
            {
               
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "AssetCategory Id not found / AssetCategory is deleted ."
                };
            }
            var assetCategories = _Imapper.Map<Core.Domain.Entities.AssetCategories>(request);
            var result = await _iAssetCategoryCommandRepository.DeleteAsync(request.Id,assetCategories);
            if (result == -1) 
            {
            
             return new ApiResponseDTO<int> { IsSuccess = false, Message = "AssetCategoryId not found."};
            }

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: assetCategories.Code,
                actionName: assetCategories.CategoryName,
                details: $"AssetCategory details was deleted",
                module: "AssetCategory");
            await _mediator.Publish(domainEvent);

            return new ApiResponseDTO<int>
            {
                IsSuccess = true,   
                Data = result,
                Message = "AssetCategory deleted successfully."
    
            };
        }
    }
}