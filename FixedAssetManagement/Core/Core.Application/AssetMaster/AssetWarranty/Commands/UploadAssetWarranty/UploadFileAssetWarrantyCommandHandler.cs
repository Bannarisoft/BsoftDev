using AutoMapper;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetWarranty;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetWarranty.Commands.UploadAssetWarranty
{
    public class UploadFileAssetWarrantyCommandHandler : IRequestHandler<UploadFileAssetWarrantyCommand, ApiResponseDTO<AssetWarrantyDTO>>
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IAssetWarrantyCommandRepository _assetWarrantyRepository;
        private readonly ILogger<UploadFileAssetWarrantyCommandHandler> _logger;

        public UploadFileAssetWarrantyCommandHandler(
            IFileUploadService fileUploadService,
            IMediator mediator,
            IMapper mapper,
            IAssetWarrantyCommandRepository assetWarrantyRepository,
            ILogger<UploadFileAssetWarrantyCommandHandler> logger)
        {
            _fileUploadService = fileUploadService;
            _mediator = mediator;
            _mapper = mapper;
            _assetWarrantyRepository = assetWarrantyRepository;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<AssetWarrantyDTO>> Handle(UploadFileAssetWarrantyCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return new ApiResponseDTO<AssetWarrantyDTO> { IsSuccess = false, Message = "No file uploaded" };
            }

            if (string.IsNullOrWhiteSpace(request.AssetCode))
            {
                return new ApiResponseDTO<AssetWarrantyDTO> { IsSuccess = false, Message = "AssetCode is required for file naming." };
            }

            // 🔹 Check if asset exists using repository
            var existingAsset = await _assetWarrantyRepository.GetByAssetCodeAsync(request.AssetCode);
            if (existingAsset == null)
            {
                return new ApiResponseDTO<AssetWarrantyDTO> { IsSuccess = false, Message = "Asset not found." };
            }

            try
            {
                // 🔹 Define Base Directory
                string baseDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "AssetWarranty");
                //string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AssetWarranty");
                EnsureDirectoryExists(baseDirectory);

                // 🔹 Construct file path
                string companyFolder = Path.Combine(baseDirectory, request.CompanyName ?? string.Empty);
                EnsureDirectoryExists(companyFolder);

                string unitFolder = Path.Combine(companyFolder, request.UnitName ?? string.Empty);
                EnsureDirectoryExists(unitFolder);

                string fileExtension = Path.GetExtension(request.File.FileName);
                string fileName = $"{request.AssetCode}{fileExtension}"; // ✅ Example: HomeTextile-COMP-MOU-1.png
                string filePath = Path.Combine(unitFolder, fileName);

                EnsureDirectoryExists(Path.GetDirectoryName(filePath));

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                // Convert Image to Base64 (optional)
                string base64Image = Convert.ToBase64String(await File.ReadAllBytesAsync(filePath));

                // ✅ Ensure correct format before saving in DB
                string formattedPath = filePath.Replace(@"\", "/");

                // ✅ Update AssetImage field using repository
                bool updateSuccess = await _assetWarrantyRepository.UpdateAssetWarrantyImageAsync(existingAsset.Id, formattedPath);
                if (!updateSuccess)
                {
                    return new ApiResponseDTO<AssetWarrantyDTO> { IsSuccess = false, Message = "Failed to update asset image." };
                }

                var response = new AssetWarrantyDTO
                {
                    Document = formattedPath,  // ✅ Correctly formatted file path
                    DocumentBase64 = base64Image  // ✅ Convert to Base64
                };

                return new ApiResponseDTO<AssetWarrantyDTO> { IsSuccess = true, Data = response };
            }
            catch (Exception ex)
            {
                _logger.LogError($"File upload failed: {ex.Message}");
                return new ApiResponseDTO<AssetWarrantyDTO> { IsSuccess = false, Message = $"File upload failed: {ex.Message}" };
            }
        }

        // ✅ Helper Method to Ensure Directory Exists
        private void EnsureDirectoryExists(string? path)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
