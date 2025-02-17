using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetSubCategories;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetSubCategories.Command.UpdateAssetSubCategories
{
    public class UpdateAssetSubCategoriesCommandHandler : IRequestHandler<UpdateAssetSubCategoriesCommand, ApiResponseDTO<int>>
    {
         private readonly IAssetSubCategoriesCommandRepository _iAssetSubCategoriesCommandRepository;
        private readonly IAssetSubCategoriesQueryRepository _iAssetSubCategoriesQueryRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _mediator; 

        public UpdateAssetSubCategoriesCommandHandler(IAssetSubCategoriesCommandRepository iAssetCategoriesCommandRepository, IAssetSubCategoriesQueryRepository iAssetCategoriesQueryRepository, IMapper imapper, IMediator mediator)
        {
            _iAssetSubCategoriesCommandRepository = iAssetCategoriesCommandRepository;
            _Imapper = imapper;
            _mediator = mediator;
            _iAssetSubCategoriesQueryRepository = iAssetCategoriesQueryRepository;
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateAssetSubCategoriesCommand request, CancellationToken cancellationToken)
        {
             // 🔹 First, check if the ID exists in the database
        var existingassetsubcategory = await _iAssetSubCategoriesQueryRepository.GetByIdAsync(request.Id);
        if (existingassetsubcategory is null)
        {
      
        return new ApiResponseDTO<int>
        {
            IsSuccess = false,
            Message = "AssetCategory Id not found / AssetCategory is deleted ."
        };
        }
         // Check for duplicate GroupName or SortOrder
       var (isNameDuplicate, isSortOrderDuplicate) = await _iAssetSubCategoriesCommandRepository
                                .CheckForDuplicatesAsync(request.SubCategoryName, request.SortOrder, request.Id);

        if (isNameDuplicate || isSortOrderDuplicate)
        {
            string errorMessage = isNameDuplicate && isSortOrderDuplicate
            ? "Both SubCategory Name and Sort Order already exist."
            : isNameDuplicate
            ? "AssetSubCategory with the same SubCategoryName already exists."
            : "AssetSubCategory with the same Sort Order already exists.";

            return new ApiResponseDTO<int>
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }
        var assetsubCategories = _Imapper.Map<Core.Domain.Entities.AssetSubCategories>(request);
        var result = await _iAssetSubCategoriesCommandRepository.UpdateAsync(request.Id, assetsubCategories);

        // AssetSubCategory not found
        {
        if (result <= 0) 
           
            return new ApiResponseDTO<int> { IsSuccess = false, Message = "AssetSubCategory id not found." };
        }

        //Domain Event
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "Update",
            actionCode: assetsubCategories.Code,
            actionName: assetsubCategories.SubCategoryName,
            details: $"AssetSubCategory details was updated",
            module: "AssetSubCategory");
        await _mediator.Publish(domainEvent, cancellationToken);
     
        return new ApiResponseDTO<int> { IsSuccess = true, Message = "Success.", Data = result };  
        }
    }
}