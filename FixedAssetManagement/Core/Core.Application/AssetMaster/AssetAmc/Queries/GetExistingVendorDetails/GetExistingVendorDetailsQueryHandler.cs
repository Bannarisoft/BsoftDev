using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetAmc;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetAmc.Queries.GetExistingVendorDetails
{
    public class GetExistingVendorDetailsQueryHandler :  IRequestHandler<GetExistingVendorDetailsQuery,ApiResponseDTO<List<GetExistingVendorDetailsDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IAssetAmcQueryRepository _iAssetAmcQueryRepository;
        public GetExistingVendorDetailsQueryHandler(IMapper mapper, IMediator mediator, IAssetAmcQueryRepository iAssetAmcQueryRepository)
        {
            _mapper = mapper;
            _mediator = mediator;
            _iAssetAmcQueryRepository = iAssetAmcQueryRepository;   
        }

        public async Task<ApiResponseDTO<List<GetExistingVendorDetailsDto>>> Handle(GetExistingVendorDetailsQuery request, CancellationToken cancellationToken)
        {
            var result = await _iAssetAmcQueryRepository.GetVendorDetails(request.OldUnitCode,request.VendorCode);
            var assetunits  = _mapper.Map<List<GetExistingVendorDetailsDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "GetAllVendorDetails",        
                    actionName: request.OldUnitCode,
                    details: $"Vendor details was fetched.",
                    module:"ExistingVendorDetails"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<GetExistingVendorDetailsDto>> { IsSuccess = true, Message = "Success", Data = assetunits };
        }
    }
}