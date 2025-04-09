

using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrderMaster.IWorkOrder;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.WorkOrder.Command.DeleteFileWorkOrder
{
    public class DeleteFileWorkOrderCommandHandler : IRequestHandler<DeleteFileWorkOrderCommand, ApiResponseDTO<bool>>
    {
        private readonly IFileUploadService _fileUploadService;        
        private readonly IWorkOrderQueryRepository _woQueryRepository;
        private readonly ILogger<DeleteFileWorkOrderCommandHandler> _logger;

        public DeleteFileWorkOrderCommandHandler(
            IFileUploadService fileUploadService,            
            IWorkOrderQueryRepository woQueryRepository,
            ILogger<DeleteFileWorkOrderCommandHandler> logger)
        {
            _fileUploadService = fileUploadService;            
            _woQueryRepository = woQueryRepository;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<bool>> Handle(DeleteFileWorkOrderCommand request, CancellationToken cancellationToken)
        { 
            string baseDirectory = await _woQueryRepository.GetBaseDirectoryAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                _logger.LogError("Base directory path not found in database.");
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Base directory not configured." };                
            }
            string companyFolder = Path.Combine(baseDirectory, request.CompanyName ?? string.Empty);
            string unitFolder = Path.Combine(companyFolder, request.UnitName ?? string.Empty);
            
            string filePath = Path.Combine(unitFolder, request.assetPath??string.Empty);

            var result = await _fileUploadService.DeleteFileAsync(filePath);
            if (result)
            {
                return new ApiResponseDTO<bool> { IsSuccess = true, Message = "File deleted successfully" };
            }
            
            return new ApiResponseDTO<bool> { IsSuccess = false, Message = "File deletion failed" };
        }
    }
}
