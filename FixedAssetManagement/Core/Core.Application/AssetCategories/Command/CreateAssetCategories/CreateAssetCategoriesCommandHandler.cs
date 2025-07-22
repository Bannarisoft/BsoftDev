using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetCategories.Queries.GetAssetCategories;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetCategories;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetCategories.Command.CreateAssetCategories
{
    public class CreateAssetCategoriesCommandHandler : IRequestHandler<CreateAssetCategoriesCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetCategoriesCommandRepository _iAssetCategoriesCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;

        public CreateAssetCategoriesCommandHandler(IAssetCategoriesCommandRepository iAssetCategoriesCommandRepository, IMediator imediator, IMapper imapper)
        {
            _iAssetCategoriesCommandRepository = iAssetCategoriesCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<int>> Handle(CreateAssetCategoriesCommand request, CancellationToken cancellationToken)
        {
            // Check if AssetGroup code already exists
            // var exists = await _iAssetCategoriesCommandRepository.ExistsByCodeAsync(request.Code);
            // if (exists)
            // {
            //     return new ApiResponseDTO<int>
            //     {
            //         IsSuccess = false,
            //         Message = "AssetCategories Code already exists.",
            //         Data = 0
            //     };
            // }
            var assetCategories = _imapper.Map<Core.Domain.Entities.AssetCategories>(request);
            var categorycode = await GenerateUniqueCodeAsync(request.CategoryName);
            assetCategories.Code = categorycode;

            var result = await _iAssetCategoriesCommandRepository.CreateAsync(assetCategories);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetCategories.Code,
                actionName: assetCategories.CategoryName,
                details: $"AssetCategories details was created",
                module: "AssetCategories");
            await _imediator.Publish(domainEvent, cancellationToken);

            var assetCategoriesDto = _imapper.Map<AssetCategoriesDto>(assetCategories);
            if (result > 0)
            {

                return new ApiResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "AssetCategories created successfully",
                    Data = result
                };
            }
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "AssetCategories Creation Failed",
                Data = result
            };
        }

         private async Task<string> GenerateUniqueCodeAsync(string categoryName)
        {
            // Take first 4 alphanumeric uppercase characters from the group name
            var baseCode = new string(categoryName
                .Where(char.IsLetterOrDigit)             // Remove special chars
                .Take(4)                                  // Take first 4
                .Select(char.ToUpper)                    // Convert to uppercase
                .ToArray());

            if (string.IsNullOrWhiteSpace(baseCode))
                baseCode = "GRP"; // Fallback if name doesn't contain valid chars

            string code = baseCode;
            int counter = 1;

            // Loop to generate unique code like COMP, COMP1, COMP2, etc.
            while (await _iAssetCategoriesCommandRepository.ExistsByCodeAsync(code))
            {
                code = $"{baseCode}{counter}";
                counter++;
            }

            return code;
        }
    }
}