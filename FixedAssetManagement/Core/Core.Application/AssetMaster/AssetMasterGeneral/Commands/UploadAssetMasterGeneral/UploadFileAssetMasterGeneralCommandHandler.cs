using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadAssetMasterGeneral
{
    public class UploadFileAssetMasterGeneralCommandHandler : IRequestHandler<UploadFileAssetMasterGeneralCommand, ApiResponseDTO<AssetMasterGeneralDTO>>
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly IAssetMasterGeneralQueryRepository _assetMasterGeneralQueryRepository;
        private readonly ILogger<UploadFileAssetMasterGeneralCommandHandler> _logger;

        public UploadFileAssetMasterGeneralCommandHandler(
            IFileUploadService fileUploadService,
            IMediator mediator,
            IMapper mapper,
            IAssetMasterGeneralCommandRepository assetMasterGeneralRepository,
            IAssetMasterGeneralQueryRepository assetMasterGeneralQueryRepository,
            ILogger<UploadFileAssetMasterGeneralCommandHandler> logger)
        {
            _fileUploadService = fileUploadService;
            _mediator = mediator;
            _mapper = mapper;
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
            _assetMasterGeneralQueryRepository = assetMasterGeneralQueryRepository;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<AssetMasterGeneralDTO>> Handle(UploadFileAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return new ApiResponseDTO<AssetMasterGeneralDTO> { IsSuccess = false, Message = "No file uploaded" };
            }
             /*       // Get latest AssetCode
            
            if (string.IsNullOrWhiteSpace(request.AssetCode))
            {
                return new ApiResponseDTO<AssetMasterGeneralDTO> { IsSuccess = false, Message = "AssetCode is required for file naming." };
            }

            // 🔹 Check if asset exists using repository
            var existingAsset = await _assetMasterGeneralRepository.GetByAssetCodeAsync(request.AssetCode);
            if (existingAsset == null)
            {
                return new ApiResponseDTO<AssetMasterGeneralDTO> { IsSuccess = false, Message = "Asset not found." };
            } */
            
            /*       // 🔹 Define Base Directory as "D:\AssetImages"
            string baseDirectory = @"D:\BSOFTImages\Asset\AssetImage";
            EnsureDirectoryExists(baseDirectory);
            */

             // 🔹 Fetch Base Directory from Database
            string baseDirectory = await _assetMasterGeneralQueryRepository.GetBaseDirectoryAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                _logger.LogError("Base directory path not found in database.");
                return new ApiResponseDTO<AssetMasterGeneralDTO> { IsSuccess = false, Message = "Base directory not configured." };
            }
            
            // 🔹 Construct the required file path
            string companyFolder = Path.Combine(baseDirectory, request.CompanyName ?? string.Empty);
            EnsureDirectoryExists(companyFolder);

            string unitFolder = Path.Combine(companyFolder, request.UnitName ?? string.Empty);
            EnsureDirectoryExists(unitFolder);

            string fileExtension = Path.GetExtension(request.File.FileName);
            //string fileName = $"{request.AssetCode}{fileExtension}";
            string dummyFileName = $"TEMP_{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(unitFolder, dummyFileName);

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
                string formattedPath = filePath.Replace(@"\", "/");
                

  /*               // ✅ Update AssetImage field using repository
                bool updateSuccess = await _assetMasterGeneralRepository.UpdateAssetImageAsync(existingAsset.Id, formattedPath);
                if (!updateSuccess)
                {
                    return new ApiResponseDTO<AssetMasterGeneralDTO> { IsSuccess = false, Message = "Failed to update asset image." };
                }
 */
                var response = new AssetMasterGeneralDTO
                {
                    AssetImage = formattedPath,  // ✅ Correctly formatted file path
                    AssetImageBase64 = base64Image  // ✅ Convert to Base64
                };

                return new ApiResponseDTO<AssetMasterGeneralDTO> { IsSuccess = true, Data = response };
            }
            catch (Exception ex)
            {
                _logger.LogError($"File upload failed: {ex.Message}");
                return new ApiResponseDTO<AssetMasterGeneralDTO> { IsSuccess = false, Message = $"File upload failed: {ex.Message}" };
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
