using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetWarranty;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.AssetMaster.AssetWarranty.Commands.DeleteFileAssetWarranty
{
    public class DeleteFileAssetWarrantyCommandHandler : IRequestHandler<DeleteFileAssetWarrantyCommand, ApiResponseDTO<bool>>
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IAssetWarrantyCommandRepository _assetWarrantyRepository;
        private readonly ILogger<DeleteFileAssetWarrantyCommandHandler> _logger;

        public DeleteFileAssetWarrantyCommandHandler(
            IFileUploadService fileUploadService,
            IAssetWarrantyCommandRepository assetWarrantyRepository,
            ILogger<DeleteFileAssetWarrantyCommandHandler> logger)
        {
            _fileUploadService = fileUploadService;
            _assetWarrantyRepository = assetWarrantyRepository;
            _logger = logger;
        }

       public async Task<ApiResponseDTO<bool>> Handle(DeleteFileAssetWarrantyCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.AssetCode))
            {
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Asset code is required." };
            }

            _logger.LogInformation("Deleting asset warranty file for AssetCode: {AssetCode}", request.AssetCode);

            // 🔹 Fetch Asset Warranty based on AssetCode
            var existingAsset = await _assetWarrantyRepository.GetByAssetWarrantyAsync(request.AssetCode);
            if (existingAsset == null || string.IsNullOrEmpty(existingAsset.Document))
            {
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Asset warranty file not found." };
            }

            string filePath = existingAsset.Document.Replace(@"\", "/");  // Normalize path
            _logger.LogInformation("File to be deleted: {FilePath}", filePath);

            // 🔹 Delete the file from storage
            var deleteResult = await _fileUploadService.DeleteFileAsync(filePath);
            if (!deleteResult)
            {
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "File deletion failed." };
            }

            // 🔹 Remove the file reference from the database
            bool updateSuccess = await _assetWarrantyRepository.RemoveAssetWarrantyAsync(existingAsset.Id);
            if (!updateSuccess)
            {
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Failed to update asset warranty record." };
            }

            _logger.LogInformation("Successfully deleted asset warranty file for AssetCode: {AssetCode}", request.AssetCode);
            return new ApiResponseDTO<bool> { IsSuccess = true, Message = "File deleted successfully." };
        }

    }
}
