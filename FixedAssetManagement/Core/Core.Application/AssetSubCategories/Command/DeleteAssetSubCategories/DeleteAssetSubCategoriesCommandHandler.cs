using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetSubCategories;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetSubCategories.Command.DeleteAssetSubCategories
{
    public class DeleteAssetSubCategoriesCommandHandler : IRequestHandler<DeleteAssetSubCategoriesCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetSubCategoriesCommandRepository _iAssetSubCategoryCommandRepository;
        private readonly IAssetSubCategoriesQueryRepository _iAssetSubCategoryQueryRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _mediator;

        public DeleteAssetSubCategoriesCommandHandler(IAssetSubCategoriesCommandRepository iAssetSubCategoryCommandRepository,IAssetSubCategoriesQueryRepository iAssetSubCategoryQueryRepository,IMapper mapper,IMediator mediator )
        {
            _Imapper=mapper;
            _mediator=mediator;
            _iAssetSubCategoryCommandRepository=iAssetSubCategoryCommandRepository;
            _iAssetSubCategoryQueryRepository=iAssetSubCategoryQueryRepository;

        }
          public async Task<ApiResponseDTO<int>> Handle(DeleteAssetSubCategoriesCommand request, CancellationToken cancellationToken)
        {
            // 🔹 First, check if the ID exists in the database
            var existingAssetCategory = await _iAssetSubCategoryQueryRepository.GetByIdAsync(request.Id);
            if (existingAssetCategory is null)
            {
               
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "AssetSubCategory Id not found / AssetSubCategory is deleted ."
                };
            }
            var assetsubCategories = _Imapper.Map<Core.Domain.Entities.AssetSubCategories>(request);
            var result = await _iAssetSubCategoryCommandRepository.DeleteAsync(request.Id,assetsubCategories);
            if (result == -1) 
            {
            
             return new ApiResponseDTO<int> { IsSuccess = false, Message = "AssetCategoryId not found."};
            }

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: assetsubCategories.Code,
                actionName: assetsubCategories.SubCategoryName,
                details: $"AssetSubCategory details was deleted",
                module: "AssetSubCategory");
            await _mediator.Publish(domainEvent);

            return new ApiResponseDTO<int>
            {
                IsSuccess = true,   
                Data = result,
                Message = "AssetSubCategory deleted successfully."
    
            };
        }
    }
}