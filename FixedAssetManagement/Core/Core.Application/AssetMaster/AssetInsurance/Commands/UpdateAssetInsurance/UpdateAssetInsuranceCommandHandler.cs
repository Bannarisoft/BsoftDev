using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetInsurance;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetInsurance.Commands.UpdateAssetInsurance
{
    public class UpdateAssetInsuranceCommandHandler  : IRequestHandler<UpdateAssetInsuranceCommand, ApiResponseDTO<bool>>
    {
    private readonly IAssetInsuranceCommandRepository _assetInsuranceCommandRepository;
    private readonly IAssetInsuranceQueryRepository _assetInsuranceQueryRepository;
    private readonly IMapper _imapper;
    private readonly IMediator _mediator;
    public UpdateAssetInsuranceCommandHandler(IAssetInsuranceCommandRepository assetInsuranceCommandRepository,IAssetInsuranceQueryRepository assetInsuranceQueryRepository, IMapper mapper, IMediator mediator)
    {
        _assetInsuranceCommandRepository = assetInsuranceCommandRepository;
        _assetInsuranceQueryRepository = assetInsuranceQueryRepository;
        _imapper = mapper;
        _mediator = mediator;

    }

     public async Task<ApiResponseDTO<bool>> Handle(UpdateAssetInsuranceCommand request, CancellationToken cancellationToken)
    {

        var assetInsurance = await _assetInsuranceQueryRepository.GetByAssetIdAsync(request.Id);
                 if (assetInsurance == null)
                {
                    return new ApiResponseDTO<bool>
                    {
                        IsSuccess = false,
                        Message = "AssetInsurance not found."
                    };
                }
                 var mAssetInsurancemap  = _imapper.Map<Core.Domain.Entities.AssetMaster.AssetInsurance>(request);         
                var AssetInsuranceresult = await _assetInsuranceCommandRepository.UpdateAsync(request.Id, mAssetInsurancemap);                

                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Update",
                        actionCode: mAssetInsurancemap.AssetId.ToString(),
                        actionName: mAssetInsurancemap.PolicyNo,
                        details: $"AssetInsurance '{mAssetInsurancemap.Id}' was updated.",
                        module:"AssetInsurance"
                    );    

                    await _mediator.Publish(domainEvent, cancellationToken); 
              
                if(AssetInsuranceresult)
                {
                    return new ApiResponseDTO<bool>{IsSuccess = true, Message = "AssetInsurance  updated successfully."};
                }

                return new ApiResponseDTO<bool>{IsSuccess = false, Message = "AssetInsurance  not updated."};


    }

        
    }
}