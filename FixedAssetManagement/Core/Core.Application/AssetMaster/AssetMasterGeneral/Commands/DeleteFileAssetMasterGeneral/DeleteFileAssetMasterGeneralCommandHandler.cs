using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteFileAssetMasterGeneral
{
    public class DeleteFileAssetMasterGeneralCommandHandler : IRequestHandler<DeleteFileAssetMasterGeneralCommand, ApiResponseDTO<bool>>
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly ILogger<DeleteFileAssetMasterGeneralCommandHandler> _logger;

        public DeleteFileAssetMasterGeneralCommandHandler(
            IFileUploadService fileUploadService,
            IAssetMasterGeneralCommandRepository assetMasterGeneralRepository,
            ILogger<DeleteFileAssetMasterGeneralCommandHandler> logger)
        {
            _fileUploadService = fileUploadService;
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<bool>> Handle(DeleteFileAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.AssetCode))
            {
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Asset image path is required." };
            }

            // 🔹 Check if asset exists by image name
            var existingAsset = await _assetMasterGeneralRepository.GetByAssetImageAsync(request.AssetCode);

            if (existingAsset == null)
            {
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Asset not found for this image." };
            }

            // 🔹 Delete the file
            var deleteResult = await _fileUploadService.DeleteFileAsync(existingAsset.AssetImage ?? string.Empty);
            if (!deleteResult)
            {
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "File deletion failed." };
            }

            // 🔹 Remove the AssetImage reference in DB
            bool updateSuccess = await _assetMasterGeneralRepository.RemoveAssetImageReferenceAsync(existingAsset.Id);
            if (!updateSuccess)
            {
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Failed to update asset record." };
            }

            _logger.LogInformation($"Asset image deleted: {request.AssetCode}");

            return new ApiResponseDTO<bool> { IsSuccess = true, Message = "File deleted successfully." };
        }
    }
}
