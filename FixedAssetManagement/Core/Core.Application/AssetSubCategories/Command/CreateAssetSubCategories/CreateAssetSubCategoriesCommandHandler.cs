using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetCategories.Command.CreateAssetCategories;
using Core.Application.AssetSubCategories.Queries.GetAssetSubCategories;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetCategories;
using Core.Application.Common.Interfaces.IAssetSubCategories;
using Core.Domain.Events;
using FluentValidation;
using MediatR;

namespace Core.Application.AssetSubCategories.Command.CreateAssetSubCategories
{
    public class CreateAssetSubCategoriesCommandHandler: IRequestHandler<CreateAssetSubCategoriesCommand, int>
    {
        private readonly IAssetSubCategoriesCommandRepository _iAssetSubCategoriesCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;

        public CreateAssetSubCategoriesCommandHandler(IAssetSubCategoriesCommandRepository iAssetSubCategoriesCommandRepository, IMediator imediator, IMapper imapper)
        {
            _iAssetSubCategoriesCommandRepository = iAssetSubCategoriesCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<int> Handle(CreateAssetSubCategoriesCommand request, CancellationToken cancellationToken)
        {
             // Check if AssetGroup code already exists
            var exists = await _iAssetSubCategoriesCommandRepository.ExistsByCodeAsync(request.Code);
            if (exists)
            {
                throw new ValidationException("AssetSubCategories Code already exists.");
              
            }
            var assetSubCategories = _imapper.Map<Core.Domain.Entities.AssetSubCategories>(request);
            
            var result = await _iAssetSubCategoriesCommandRepository.CreateAsync(assetSubCategories);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetSubCategories.Code,
                actionName: assetSubCategories.SubCategoryName,
                details: $"AssetSubCategories details was created",
                module: "AssetSubCategories");
            await _imediator.Publish(domainEvent, cancellationToken);
          
            
            if (result > 0)
                  {
                   
                        return  result;
                 }
                 throw new Exception("AssetSubCategories Creation Failed");
            
        }
    }
}