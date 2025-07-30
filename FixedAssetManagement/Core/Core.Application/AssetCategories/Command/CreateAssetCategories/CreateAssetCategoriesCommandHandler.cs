using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetCategories.Queries.GetAssetCategories;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetCategories;
using Core.Domain.Events;
using FluentValidation;
using MediatR;

namespace Core.Application.AssetCategories.Command.CreateAssetCategories
{
    public class CreateAssetCategoriesCommandHandler : IRequestHandler<CreateAssetCategoriesCommand, int>
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

        public async Task<int> Handle(CreateAssetCategoriesCommand request, CancellationToken cancellationToken)
        {
           // Check if AssetGroup code already exists
            var exists = await _iAssetCategoriesCommandRepository.ExistsByCodeAsync(request.Code);
            if (exists)
            {
                throw new ValidationException("AssetCategories Code already exists.");
            }
            var assetCategories = _imapper.Map<Core.Domain.Entities.AssetCategories>(request);
            
            var result = await _iAssetCategoriesCommandRepository.CreateAsync(assetCategories);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetCategories.Code,
                actionName: assetCategories.CategoryName,
                details: $"AssetCategories details was created",
                module: "AssetCategories");
            await _imediator.Publish(domainEvent, cancellationToken);
          
            
            if (result <= 0)
             {
              
               throw new Exception("AssetCategories Creation Failed");
             }
             return result;
            
        }
    }
}