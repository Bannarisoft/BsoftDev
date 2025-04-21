

using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.WorkOrder.Command.UploadFileWorOrder
{
    public class UploadFileWorkOrderCommandHandler : IRequestHandler<UploadFileWorkOrderCommand, ApiResponseDTO<WorkOrderImageDto>>
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IWorkOrderCommandRepository _woCommandRepository;
        private readonly IWorkOrderQueryRepository _woQueryRepository;
        private readonly ILogger<UploadFileWorkOrderCommandHandler> _logger;

        public UploadFileWorkOrderCommandHandler(
            IFileUploadService fileUploadService,
            IMediator mediator,
            IMapper mapper,
            IWorkOrderCommandRepository woCommandRepository,
            IWorkOrderQueryRepository woQueryRepository,
            ILogger<UploadFileWorkOrderCommandHandler> logger)
        {
            _fileUploadService = fileUploadService;
            _mediator = mediator;
            _mapper = mapper;
            _woCommandRepository = woCommandRepository;
            _woQueryRepository = woQueryRepository;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<WorkOrderImageDto>> Handle(UploadFileWorkOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return new ApiResponseDTO<WorkOrderImageDto> { IsSuccess = false, Message = "No file uploaded" };
            }           
             // 🔹 Fetch Base Directory from Database
            string baseDirectory = await _woQueryRepository.GetBaseDirectoryAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
               _logger.LogError("Base directory path not found in database.");
                return new ApiResponseDTO<WorkOrderImageDto> { IsSuccess = false, Message = "Base directory not configured." };
            }
            
            // 🔹 Construct the required file path
          /*   string companyFolder = Path.Combine(baseDirectory, request.CompanyName?.Trim() ?? string.Empty);
            EnsureDirectoryExists(companyFolder);

            string unitFolder = Path.Combine(companyFolder, request.UnitName?.Trim() ?? string.Empty);
            EnsureDirectoryExists(unitFolder); */

            string fileExtension = Path.GetExtension(request.File.FileName);            
            string dummyFileName = $"TEMP_{Guid.NewGuid()}{fileExtension}";
            //string filePath = Path.Combine(unitFolder, dummyFileName);
            string filePath = Path.Combine("", dummyFileName);

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
                 var response = new WorkOrderImageDto
                {
                    WorkOrderImage = formattedPath,  // ✅ Correctly formatted file path
                    WorkOrderImageBase64 = base64Image  // ✅ Convert to Base64
                };

                return new ApiResponseDTO<WorkOrderImageDto> { IsSuccess = true, Data = response };
            }
            catch (Exception ex)
            {
                _logger.LogError($"File upload failed: {ex.Message}");
                return new ApiResponseDTO<WorkOrderImageDto> { IsSuccess = false, Message = $"File upload failed: {ex.Message}" };
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
