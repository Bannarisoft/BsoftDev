using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteFileAssetMasterGeneral
{
    public class DeleteFileAssetMasterGeneralCommandHandler : IRequestHandler<DeleteFileAssetMasterGeneralCommand, ApiResponseDTO<bool>>
    {
        private readonly IFileUploadService _iFileUploadService;
        public DeleteFileAssetMasterGeneralCommandHandler(IFileUploadService iFileUploadService)
        {
            _iFileUploadService = iFileUploadService;
        }
       public async Task<ApiResponseDTO<bool>> Handle(DeleteFileAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {
            var result = await _iFileUploadService.DeleteFileAsync(request.AssetImage ?? string.Empty);
            return new ApiResponseDTO<bool>{IsSuccess = true, Message = "File deleted successfully"};
        }
    }
}