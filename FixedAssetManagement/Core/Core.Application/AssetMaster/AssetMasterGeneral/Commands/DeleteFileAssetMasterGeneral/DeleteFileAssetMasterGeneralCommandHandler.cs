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

        public DeleteFileAssetMasterGeneralCommandHandler(
            IFileUploadService fileUploadService,            
            IAssetMasterGeneralQueryRepository assetMasterGeneralQueryRepository,
            ILogger<DeleteFileAssetMasterGeneralCommandHandler> logger)
        {
            _fileUploadService = fileUploadService;            
            _assetMasterGeneralQueryRepository = assetMasterGeneralQueryRepository;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<bool>> Handle(DeleteFileAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        { 
            string baseDirectory = await _assetMasterGeneralQueryRepository.GetBaseDirectoryAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                _logger.LogError("Base directory path not found in database.");
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Base directory not configured." };                
            }
            
            string companyFolder = Path.Combine(baseDirectory, request.CompanyName ?? string.Empty);
            string unitFolder = Path.Combine(companyFolder, request.UnitName ?? string.Empty);
            
            string filePath = Path.Combine(unitFolder, request.assetPath??string.Empty);

            //var correctedAssetPath = request.assetPath?.Replace("\\", "\\\\") ?? string.Empty;         

            var result = await _fileUploadService.DeleteFileAsync(filePath);
            if (result)
            {
                return new ApiResponseDTO<bool> { IsSuccess = true, Message = "File deleted successfully" };
            }
            return new ApiResponseDTO<bool> { IsSuccess = false, Message = "File deletion failed" };
        }
    }
}
