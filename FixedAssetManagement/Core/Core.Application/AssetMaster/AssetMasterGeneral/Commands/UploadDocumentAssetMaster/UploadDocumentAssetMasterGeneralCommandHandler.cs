using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadDocumentAssetMaster
{
    public class UploadDocumentAssetMasterGeneralCommandHandler : IRequestHandler<UploadDocumentAssetMasterGeneralCommand, ApiResponseDTO<AssetMasterDocumentDto>>
    {        
        private readonly IAssetMasterGeneralQueryRepository _assetMasterGeneralQueryRepository;
        private readonly ILogger<UploadDocumentAssetMasterGeneralCommandHandler> _logger;
        private readonly IIPAddressService _ipAddressService;
        private readonly IUnitGrpcClient _unitGrpcClient;
        private readonly ICompanyGrpcClient _companyGrpcClient;

        public UploadDocumentAssetMasterGeneralCommandHandler(          
            IAssetMasterGeneralQueryRepository assetMasterGeneralQueryRepository,
            ILogger<UploadDocumentAssetMasterGeneralCommandHandler> logger, IIPAddressService ipAddressService, IUnitGrpcClient unitGrpcClient, ICompanyGrpcClient companyGrpcClient)
        {          
            _assetMasterGeneralQueryRepository = assetMasterGeneralQueryRepository;
            _logger = logger;
            _ipAddressService = ipAddressService;
            _unitGrpcClient = unitGrpcClient;
            _companyGrpcClient = companyGrpcClient;
        }

        public async Task<ApiResponseDTO<AssetMasterDocumentDto>> Handle(UploadDocumentAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return new ApiResponseDTO<AssetMasterDocumentDto> { IsSuccess = false, Message = "No file uploaded" };
            }
             // 🔹 Fetch Base Directory from Database
            string baseDirectory = await _assetMasterGeneralQueryRepository.GetDocumentDirectoryAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                _logger.LogError("Base directory path not found in database.");
                return new ApiResponseDTO<AssetMasterDocumentDto> { IsSuccess = false, Message = "Base directory not configured." };
            }
            
            var companyId =_ipAddressService.GetCompanyId();
            var unitId = _ipAddressService.GetUnitId();

            var companies = await _companyGrpcClient.GetAllCompanyAsync();
            var units = await _unitGrpcClient.GetAllUnitAsync();

            var companyLookup = companies.ToDictionary(c => c.CompanyId, c => c.CompanyName);
            var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);

            var companyName = companyLookup.TryGetValue(companyId, out var cname) ? cname : string.Empty;
            var unitName = unitLookup.TryGetValue(unitId, out var uname) ? uname : string.Empty;            
            
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory,companyName,unitName);                
            EnsureDirectoryExists(uploadPath);

            string fileExtension = Path.GetExtension(request.File.FileName);            
            string dummyFileName = $"TEMP_{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(uploadPath, dummyFileName);

            try
            {
                EnsureDirectoryExists(Path.GetDirectoryName(filePath));

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                // Convert Image to Base64 (optional)
                string base64Image = Convert.ToBase64String(await File.ReadAllBytesAsync(filePath));

                // ✅ Ensure the correct format before saving in DB
                string formattedPath = dummyFileName;

                var response = new AssetMasterDocumentDto
                {
                    AssetDocument = formattedPath,  // ✅ Correctly formatted file path
                    AssetDocumentBase64 = base64Image  // ✅ Convert to Base64
                };
                return new ApiResponseDTO<AssetMasterDocumentDto> { IsSuccess = true, Data = response };
            }
            catch (Exception ex)
            {
                _logger.LogError($"File upload failed: {ex.Message}");
                return new ApiResponseDTO<AssetMasterDocumentDto> { IsSuccess = false, Message = $"File upload failed: {ex.Message}" };
            }
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
