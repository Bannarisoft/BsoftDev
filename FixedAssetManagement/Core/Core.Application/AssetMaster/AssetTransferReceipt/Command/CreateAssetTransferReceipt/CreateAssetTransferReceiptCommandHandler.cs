using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Command.CreateAssetTransferReceipt
{
    public class CreateAssetTransferReceiptCommandHandler : IRequestHandler<CreateAssetTransferReceiptCommand, ApiResponseDTO<int>>
    {
        // private readonly IAssetDisposalCommandRepository _iassetdisposalcommandrepository;
        // private readonly IMediator _imediator;
        // private readonly IMapper _imapper;
        public Task<ApiResponseDTO<int>> Handle(CreateAssetTransferReceiptCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}