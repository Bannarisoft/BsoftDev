

using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadDocumentAssetMaster;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.SaveAssetDocument
{
    public class SaveAssetDocumentCommandHandler : IRequestHandler<SaveAssetDocumentCommand, ApiResponseDTO<bool>>
    {
        private readonly IAssetMasterGeneralQueryRepository _assetMasterGeneralQueryRepository;
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly ILogger<UploadDocumentAssetMasterGeneralCommandHandler> _logger;
        private readonly IIPAddressService _ipAddressService;

          public SaveAssetDocumentCommandHandler(          
            IAssetMasterGeneralQueryRepository assetMasterGeneralQueryRepository,
            ILogger<UploadDocumentAssetMasterGeneralCommandHandler> logger, IIPAddressService ipAddressService,IAssetMasterGeneralCommandRepository assetMasterGeneralRepository)
        {          
            _assetMasterGeneralQueryRepository = assetMasterGeneralQueryRepository;
            _logger = logger;
            _ipAddressService = ipAddressService;
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
        }

        public async Task<ApiResponseDTO<bool>> Handle(SaveAssetDocumentCommand request, CancellationToken cancellationToken)
        {
             if (request.assetPath == null || request.assetPath.Length == 0)
            {
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "No file uploaded" };
            }

            string tempFilePath = request.assetPath;
            if (tempFilePath != null){
                string baseDirectory = await _assetMasterGeneralQueryRepository.GetDocumentDirectoryAsync();                
                var companyId =_ipAddressService.GetCompanyId();
                var unitId = _ipAddressService.GetUnitId();

                var (companyName, unitName) = await _assetMasterGeneralQueryRepository.GetCompanyUnitAsync(companyId, unitId);
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory,companyName,unitName);    

                string filePath = Path.Combine(uploadPath, tempFilePath);  
                EnsureDirectoryExists(Path.GetDirectoryName(filePath));           

                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
                    string newFileName = $"{request.AssetCode}{Path.GetExtension(tempFilePath)}";
                    string newFilePath = Path.Combine(directory, newFileName);

                    try
                    {
                        File.Move(filePath, newFilePath);
                        //assetEntity.AssetImage = newFileName;
                        await _assetMasterGeneralRepository.UpdateDocumentAsync(request.Id, newFileName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to rename file: {ex.Message}");
                    }
                }
            }                    
            return new ApiResponseDTO<bool>
            {
                IsSuccess = true,
                Message = "WorkOrder updated."                     
            };
        }   
        private void EnsureDirectoryExists(string path)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}