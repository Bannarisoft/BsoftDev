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
        private readonly ILogger<UploadFileAssetMasterGeneralCommandHandler> _logger;

        public UploadFileAssetMasterGeneralCommandHandler(
            IFileUploadService fileUploadService,
            IMediator mediator,
            IMapper mapper,
            IAssetMasterGeneralCommandRepository assetMasterGeneralRepository,
            ILogger<UploadFileAssetMasterGeneralCommandHandler> logger)
        {
            _fileUploadService = fileUploadService;
            _mediator = mediator;
            _mapper = mapper;
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
            _logger = logger;
        }

       public async Task<ApiResponseDTO<AssetMasterGeneralDTO>> Handle(UploadFileAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return new ApiResponseDTO<AssetMasterGeneralDTO> { IsSuccess = false, Message = "No file uploaded" };
            }

            if (string.IsNullOrWhiteSpace(request.AssetCode))
            {
                return new ApiResponseDTO<AssetMasterGeneralDTO> { IsSuccess = false, Message = "AssetCode is required for file naming." };
            }

            // 🔹 Check if asset exists using repository
            var existingAsset = await _assetMasterGeneralRepository.GetByAssetCodeAsync(request.AssetCode);
            if (existingAsset == null)
            {
                return new ApiResponseDTO<AssetMasterGeneralDTO> { IsSuccess = false, Message = "Asset not found." };
            }

            // 🔹 Define Base Directory (Get parent directory)
            
            //string baseDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "AssetImages");
            string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "AssetImages");
            EnsureDirectoryExists(baseDirectory);

            // 🔹 Construct the required file path
            string companyFolder = Path.Combine(baseDirectory, request.CompanyName ?? string.Empty);
            EnsureDirectoryExists(companyFolder);

            string unitFolder = Path.Combine(companyFolder, request.UnitName ?? string.Empty);
            EnsureDirectoryExists(unitFolder);

            string fileExtension = Path.GetExtension(request.File.FileName);
            string fileName = $"{request.AssetCode}{fileExtension}";  // ✅ Example: HomeTextile-COMP-MOU-1.png
            string filePath = Path.Combine(unitFolder, fileName);

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

                // ✅ Update AssetImage field using repository
                bool updateSuccess = await _assetMasterGeneralRepository.UpdateAssetImageAsync(existingAsset.Id, formattedPath);
                if (!updateSuccess)
                {
                    return new ApiResponseDTO<AssetMasterGeneralDTO> { IsSuccess = false, Message = "Failed to update asset image." };
                }

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
        // ✅ Helper Method to Ensure Directory Exists
        private void EnsureDirectoryExists(string path)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

    }
}
