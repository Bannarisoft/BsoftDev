using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetCategories;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetCategories.Command.UpdateAssetCategories
{
    public class UpdateAssetCategoriesCommandHandler : IRequestHandler<UpdateAssetCategoriesCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetCategoriesCommandRepository _iAssetCategoriesCommandRepository;
        private readonly IAssetCategoriesQueryRepository _iAssetCategoriesQueryRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _mediator; 

        public UpdateAssetCategoriesCommandHandler( IAssetCategoriesCommandRepository iAssetCategoriesCommandRepository, IAssetCategoriesQueryRepository iAssetCategoriesQueryRepository, IMapper imapper, IMediator mediator)
        {
            _iAssetCategoriesCommandRepository = iAssetCategoriesCommandRepository;
            _Imapper = imapper;
            _mediator = mediator;
            _iAssetCategoriesQueryRepository = iAssetCategoriesQueryRepository;
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateAssetCategoriesCommand request, CancellationToken cancellationToken)
        {
        // 🔹 First, check if the ID exists in the database
        var existingassetcategory = await _iAssetCategoriesQueryRepository.GetByIdAsync(request.Id);
        if (existingassetcategory is null)
        {
      
        return new ApiResponseDTO<int>
        {
            IsSuccess = false,
            Message = "AssetCategory Id not found / AssetCategory is deleted ."
        };
        }
         // Check for duplicate GroupName or SortOrder
       var (isNameDuplicate, isSortOrderDuplicate) = await _iAssetCategoriesCommandRepository
                                .CheckForDuplicatesAsync(request.CategoryName, request.SortOrder, request.Id);

        if (isNameDuplicate || isSortOrderDuplicate)
        {
            string errorMessage = isNameDuplicate && isSortOrderDuplicate
            ? "Both Category Name and Sort Order already exist."
            : isNameDuplicate
            ? "AssetCategory with the same CategoryName already exists."
            : "AssetCategory with the same Sort Order already exists.";

            return new ApiResponseDTO<int>
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }
        var assetCategories = _Imapper.Map<Core.Domain.Entities.AssetCategories>(request);
        var result = await _iAssetCategoriesCommandRepository.UpdateAsync(request.Id, assetCategories);

        // AssetGroup not found
        {
        if (result <= 0) 
           
            return new ApiResponseDTO<int> { IsSuccess = false, Message = "AssetGroup not found." };
        }

        //Domain Event
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "Update",
            actionCode: assetCategories.Code,
            actionName: assetCategories.CategoryName,
            details: $"AssetCategory details was updated",
            module: "AssetCategory");
        await _mediator.Publish(domainEvent, cancellationToken);
     
        return new ApiResponseDTO<int> { IsSuccess = true, Message = "Success.", Data = result };  
        }
    }      
    }
