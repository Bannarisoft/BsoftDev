using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadAssetMasterGeneral
{
    public class UploadFileAssetMasterGeneralCommandHandler : IRequestHandler<UploadFileAssetMasterGeneralCommand, ApiResponseDTO<AssetMasterImageDto>>
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly IAssetMasterGeneralQueryRepository _assetMasterGeneralQueryRepository;
        private readonly ILogger<UploadFileAssetMasterGeneralCommandHandler> _logger;
        private readonly IIPAddressService _ipAddressService;

        public UploadFileAssetMasterGeneralCommandHandler(
            IFileUploadService fileUploadService,
            IMediator mediator,
            IMapper mapper,
            IAssetMasterGeneralCommandRepository assetMasterGeneralRepository,
            IAssetMasterGeneralQueryRepository assetMasterGeneralQueryRepository,
            ILogger<UploadFileAssetMasterGeneralCommandHandler> logger, IIPAddressService ipAddressService)
        {
            _fileUploadService = fileUploadService;
            _mediator = mediator;
            _mapper = mapper;
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
            _assetMasterGeneralQueryRepository = assetMasterGeneralQueryRepository;
            _logger = logger;
            _ipAddressService = ipAddressService;
        }

        public async Task<ApiResponseDTO<AssetMasterImageDto>> Handle(UploadFileAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return new ApiResponseDTO<AssetMasterImageDto> { IsSuccess = false, Message = "No file uploaded" };
            }

             // 🔹 Fetch Base Directory from Database
            string baseDirectory = await _assetMasterGeneralQueryRepository.GetBaseDirectoryAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                _logger.LogError("Base directory path not found in database.");
                return new ApiResponseDTO<AssetMasterImageDto> { IsSuccess = false, Message = "Base directory not configured." };
            }
            
            var companyId =_ipAddressService.GetCompanyId();
            var unitId = _ipAddressService.GetUnitId();
            var (companyName, unitName) = await _assetMasterGeneralQueryRepository.GetCompanyUnitAsync(companyId, unitId);
            
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

                var response = new AssetMasterImageDto
                {
                    AssetImage = formattedPath,  // ✅ Correctly formatted file path
                    AssetImageBase64 = base64Image  // ✅ Convert to Base64
                };
                return new ApiResponseDTO<AssetMasterImageDto> { IsSuccess = true, Data = response };
            }
            catch (Exception ex)
            {
                _logger.LogError($"File upload failed: {ex.Message}");
                return new ApiResponseDTO<AssetMasterImageDto> { IsSuccess = false, Message = $"File upload failed: {ex.Message}" };
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
