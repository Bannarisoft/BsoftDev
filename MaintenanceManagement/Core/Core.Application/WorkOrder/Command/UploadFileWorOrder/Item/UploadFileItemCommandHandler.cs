
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.WorkOrder.Command.UploadFileWorOrder.Item
{
    public class UploadFileItemCommandHandler : IRequestHandler<UploadFileItemCommand, ApiResponseDTO<ItemImageDto>>
    {
        
        private readonly ILogger<UploadFileWorkOrderCommandHandler> _logger;
         private readonly IIPAddressService _ipAddressService;
         private readonly IWorkOrderCommandRepository _workOrderRepository;

        public UploadFileItemCommandHandler(                       
            ILogger<UploadFileWorkOrderCommandHandler> logger, IIPAddressService ipAddressService,IWorkOrderCommandRepository workOrderRepository)
        {               
            _logger = logger;
            _ipAddressService = ipAddressService;
            _workOrderRepository = workOrderRepository;
        }

        public async Task<ApiResponseDTO<ItemImageDto>> Handle(UploadFileItemCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return new ApiResponseDTO<ItemImageDto> { IsSuccess = false, Message = "No file uploaded" };
            }           
             // 🔹 Fetch Base Directory from Database
            string baseDirectory = await _workOrderRepository.GetBaseDirectoryItemAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
               _logger.LogError("Base directory path not found in database.");
                return new ApiResponseDTO<ItemImageDto> { IsSuccess = false, Message = "Base directory not configured." };
            }
            var companyId =1; //_ipAddressService.GetCompanyId();
            var unitId = 41;//_ipAddressService.GetUnitId();
            var (companyName, unitName) = await _workOrderRepository.GetCompanyUnitAsync(companyId, unitId);

            // 🔹 Construct the required file path
             string companyFolder = Path.Combine(baseDirectory,companyName);
            EnsureDirectoryExists(companyFolder);

            string unitFolder = Path.Combine(companyFolder,unitName);
            EnsureDirectoryExists(unitFolder); 

            string fileExtension = Path.GetExtension(request.File.FileName);            
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
                string formattedPath = dummyFileName;
                 var response = new ItemImageDto
                {
                    WorkOrderItemImage = formattedPath,  // ✅ Correctly formatted file path
                    WorkOrderImageItemBase64 = base64Image  // ✅ Convert to Base64
                };

                return new ApiResponseDTO<ItemImageDto> { IsSuccess = true, Data = response };
            }
            catch (Exception ex)
            {
                _logger.LogError($"File upload failed: {ex.Message}");
                return new ApiResponseDTO<ItemImageDto> { IsSuccess = false, Message = $"File upload failed: {ex.Message}" };
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
