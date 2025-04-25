
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.WorkOrder.Command.DeleteFileWorkOrder.Item
{
    public class DeleteFileItemCommandHandler : IRequestHandler<DeleteFileItemCommand, ApiResponseDTO<bool>>
    {
        private readonly IFileUploadService _fileUploadService;        
        private readonly IWorkOrderQueryRepository _woQueryRepository;        
        private readonly IIPAddressService _ipAddressService;
        private readonly IWorkOrderCommandRepository _workOrderRepository;

        public DeleteFileItemCommandHandler(
            IFileUploadService fileUploadService,            
            IWorkOrderQueryRepository woQueryRepository,
            ILogger<DeleteFileWorkOrderCommandHandler> logger, IIPAddressService ipAddressService,IWorkOrderCommandRepository workOrderRepository)
        {
            _fileUploadService = fileUploadService;            
            _woQueryRepository = woQueryRepository;            
            _ipAddressService = ipAddressService;
            _workOrderRepository = workOrderRepository;
        }

        public async Task<ApiResponseDTO<bool>> Handle(DeleteFileItemCommand request, CancellationToken cancellationToken)
        { 
            var companyId = 1;//_ipAddressService.GetCompanyId();
            var unitId = 41;//_ipAddressService.GetUnitId();
            var (companyName, unitName) = await _workOrderRepository.GetCompanyUnitAsync(companyId, unitId);

            string baseDirectory = await _workOrderRepository.GetBaseDirectoryItemAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {                
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Base directory not configured." };                
            }
            string companyFolder = Path.Combine(baseDirectory, companyName);
            string unitFolder = Path.Combine(companyFolder, unitName);
            
            string filePath = Path.Combine(unitFolder, request.Image??string.Empty);

            var result = await _fileUploadService.DeleteFileAsync(filePath);

            await _workOrderRepository.DeleteItemImageAsync( request.Image);

            if (result)
            {
                return new ApiResponseDTO<bool> { IsSuccess = true, Message = "File deleted successfully" };
            }            
            return new ApiResponseDTO<bool> { IsSuccess = false, Message = "File deletion failed" };
        }
    }
}