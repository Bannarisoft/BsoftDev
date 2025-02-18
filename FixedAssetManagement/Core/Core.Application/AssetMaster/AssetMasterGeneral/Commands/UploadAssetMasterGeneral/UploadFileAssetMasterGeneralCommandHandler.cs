using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadAssetMasterGeneral
{
    public class UploadFileAssetMasterGeneralCommandHandler : IRequestHandler<UploadFileAssetMasterGeneralCommand, ApiResponseDTO<AssetMasterGeneralDTO>>
    {
        private readonly IFileUploadService _iFileUploadService;
         private readonly IMediator _mediator;
         private readonly IMapper _iMapper;

        public UploadFileAssetMasterGeneralCommandHandler(IFileUploadService iFileUploadService, IMediator mediator, IMapper iMapper)
        {
            _iFileUploadService = iFileUploadService;
            _mediator = mediator;
            _iMapper = iMapper;
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


            // Define the base directory outside the project root
            string baseDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "AssetImages");            
            EnsureDirectoryExists(baseDirectory);         
            string companyFolder = Path.Combine(baseDirectory, request.CompanyName??string.Empty);
            EnsureDirectoryExists(companyFolder);
            string unitFolder = Path.Combine(companyFolder, request.UnitName??string.Empty);            
            EnsureDirectoryExists(unitFolder);
         
            string fileExtension = Path.GetExtension(request.File.FileName);
            string fileName = $"{request.AssetCode}{fileExtension}";  // ✅ Use AssetCode as filename
            string filePath = Path.Combine(unitFolder, fileName);
            try
            {
                 EnsureDirectoryExists(Path.GetDirectoryName(filePath));
                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }
              
                // Convert Image to Base64 (optional)
                string base64Image = Convert.ToBase64String(await File.ReadAllBytesAsync(filePath));

                var response = new AssetMasterGeneralDTO
                {
                    AssetImage = filePath,  // ✅ Store file path
                    AssetImageBase64 = base64Image  // ✅ Convert to Base64
                };
          //      existingAsset.AssetImage = fileName;  // ✅ Store only the filename, not full path
        //        await _applicationDbContext.SaveChangesAsync();
                return new ApiResponseDTO<AssetMasterGeneralDTO> { IsSuccess = true, Data = response };
            }
            catch (Exception ex)
            {
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