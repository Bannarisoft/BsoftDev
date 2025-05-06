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
        private readonly IIPAddressService _ipAddressService;
        private readonly IAssetMasterGeneralQueryRepository _assetMasterGeneralRepository;
        private readonly IAssetWarrantyQueryRepository _assetWarrantQueryRepository;

        public DeleteFileAssetWarrantyCommandHandler(
            IFileUploadService fileUploadService,
            IAssetWarrantyCommandRepository assetWarrantyRepository,
            ILogger<DeleteFileAssetWarrantyCommandHandler> logger, IIPAddressService ipAddressService,IAssetMasterGeneralQueryRepository assetMasterGeneralRepository,IAssetWarrantyQueryRepository assetWarrantQueryRepository)
        {
            _fileUploadService = fileUploadService;
            _assetWarrantyRepository = assetWarrantyRepository;
            _logger = logger; _ipAddressService = ipAddressService;_assetMasterGeneralRepository=assetMasterGeneralRepository;_assetWarrantQueryRepository=assetWarrantQueryRepository;
        }

       public async Task<ApiResponseDTO<bool>> Handle(DeleteFileAssetWarrantyCommand request, CancellationToken cancellationToken)
        {
            var companyId = _ipAddressService.GetCompanyId();
            var unitId = _ipAddressService.GetUnitId();
            var (companyName, unitName) = await _assetMasterGeneralRepository.GetCompanyUnitAsync(companyId, unitId);
            
            string baseDirectory = await _assetWarrantQueryRepository.GetBaseDirectoryAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                _logger.LogError("Base directory path not found in database.");
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Base directory not configured." };                
            }
            
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory,companyName,unitName);       

            string filePath = Path.Combine(uploadPath, request.assetPath??string.Empty);

            var result = await _fileUploadService.DeleteFileAsync(filePath);

            await _assetWarrantyRepository.RemoveAssetWarrantyAsync(request.assetPath);
              if (result)
            {
                return new ApiResponseDTO<bool> { IsSuccess = true, Message = "File deleted successfully" };
            }
            return new ApiResponseDTO<bool> { IsSuccess = false, Message = "File deletion failed" };
        }

    }
}
