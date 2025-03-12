using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssertByCategory
{
    public class GetAssetsByCategoryQueryHandler  : IRequestHandler<GetAssetsByCategoryQuery, List<GetAssetMasterDto>>
    {


        private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;
        private readonly IMapper _mapper;
        public GetAssetsByCategoryQueryHandler( IAssetTransferQueryRepository assetTransferQueryRepository, IMapper mapper)
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
            _mapper = mapper;
        }
        public async Task<List<GetAssetMasterDto>> Handle(GetAssetsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var assets = await _assetTransferQueryRepository.GetAssetsByCategoryAsync(request.AssetCategoryId);

            return _mapper.Map<List<GetAssetMasterDto>>(assets);
        }


        
    }
}