using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.WorkOrder.Command.DeleteFileWorkOrder
{
    public class DeleteFileWorkOrderCommandHandler : IRequestHandler<DeleteFileWorkOrderCommand, ApiResponseDTO<bool>>
    {
        private readonly IFileUploadService _fileUploadService;        
        private readonly IWorkOrderQueryRepository _woQueryRepository;
        private readonly ILogger<DeleteFileWorkOrderCommandHandler> _logger;
        private readonly IIPAddressService _ipAddressService;
        private readonly IWorkOrderCommandRepository _workOrderRepository;        
        private readonly IUnitGrpcClient _unitGrpcClient;
        private readonly ICompanyGrpcClient _companyGrpcClient;

        public DeleteFileWorkOrderCommandHandler(
            IFileUploadService fileUploadService,            
            IWorkOrderQueryRepository woQueryRepository,
            ILogger<DeleteFileWorkOrderCommandHandler> logger, IIPAddressService ipAddressService,IWorkOrderCommandRepository workOrderRepository,IUnitGrpcClient unitGrpcClient, ICompanyGrpcClient companyGrpcClient)
        {
            _fileUploadService = fileUploadService;            
            _woQueryRepository = woQueryRepository;
            _logger = logger;
            _ipAddressService = ipAddressService;
            _workOrderRepository = workOrderRepository;
            _unitGrpcClient = unitGrpcClient;
            _companyGrpcClient = companyGrpcClient;
        }

        public async Task<ApiResponseDTO<bool>> Handle(DeleteFileWorkOrderCommand request, CancellationToken cancellationToken)
        { 
            var companyId = _ipAddressService.GetCompanyId();
            var unitId = _ipAddressService.GetUnitId();
             var companies = await _companyGrpcClient.GetAllCompanyAsync();
            var units = await _unitGrpcClient.GetAllUnitAsync();

            var companyLookup = companies.ToDictionary(c => c.CompanyId, c => c.CompanyName);
            var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);

            var companyName = companyLookup.TryGetValue(companyId, out var cname) ? cname : string.Empty;
            var unitName = unitLookup.TryGetValue(unitId, out var uname) ? uname : string.Empty;   

            string baseDirectory = await _woQueryRepository.GetBaseDirectoryAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                _logger.LogError("Base directory path not found in database.");
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Base directory not configured." };                
            }
             string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory,companyName,unitName);       

            string filePath = Path.Combine(uploadPath, request.Image??string.Empty);

            var result = await _fileUploadService.DeleteFileAsync(filePath);

            await _workOrderRepository.DeleteWOImageAsync( request.Image);

            if (result)
            {
                return new ApiResponseDTO<bool> { IsSuccess = true, Message = "File deleted successfully" };
            }            
            return new ApiResponseDTO<bool> { IsSuccess = false, Message = "File deletion failed" };
        }
    }
}
