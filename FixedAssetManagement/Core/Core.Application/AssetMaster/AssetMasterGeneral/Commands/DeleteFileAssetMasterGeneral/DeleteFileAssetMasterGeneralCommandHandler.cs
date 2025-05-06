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
        private readonly IAssetMasterGeneralQueryRepository _assetMasterGeneralQueryRepository;
        private readonly ILogger<DeleteFileAssetMasterGeneralCommandHandler> _logger;
        private readonly IIPAddressService _ipAddressService;
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;

        public DeleteFileAssetMasterGeneralCommandHandler(
            IFileUploadService fileUploadService,            
            IAssetMasterGeneralQueryRepository assetMasterGeneralQueryRepository,
            ILogger<DeleteFileAssetMasterGeneralCommandHandler> logger, IIPAddressService ipAddressService,IAssetMasterGeneralCommandRepository assetMasterGeneralRepository)
        {
            _fileUploadService = fileUploadService;            
            _assetMasterGeneralQueryRepository = assetMasterGeneralQueryRepository;
            _logger = logger;  _ipAddressService = ipAddressService;_assetMasterGeneralRepository=assetMasterGeneralRepository;
        }

        public async Task<ApiResponseDTO<bool>> Handle(DeleteFileAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        { 
            var companyId = _ipAddressService.GetCompanyId();
            var unitId = _ipAddressService.GetUnitId();
            var (companyName, unitName) = await _assetMasterGeneralQueryRepository.GetCompanyUnitAsync(companyId, unitId);
            
            string baseDirectory = await _assetMasterGeneralQueryRepository.GetBaseDirectoryAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                _logger.LogError("Base directory path not found in database.");
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Base directory not configured." };                
            }
            
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory,companyName,unitName);       

            string filePath = Path.Combine(uploadPath, request.assetPath??string.Empty);

            var result = await _fileUploadService.DeleteFileAsync(filePath);

            await _assetMasterGeneralRepository.RemoveAssetImageReferenceAsync(request.assetPath);
            if (result)
            {
                return new ApiResponseDTO<bool> { IsSuccess = true, Message = "File deleted successfully" };
            }
            return new ApiResponseDTO<bool> { IsSuccess = false, Message = "File deletion failed" };
        }
    }
}
