using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetWarranty;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.AssetMaster.AssetWarranty.Commands.DeleteFileAssetWarranty
{
    public class DeleteFileAssetWarrantyCommandHandler : IRequestHandler<DeleteFileAssetWarrantyCommand, ApiResponseDTO<bool>>
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IAssetWarrantyCommandRepository _assetWarrantyRepository;
        private readonly ILogger<DeleteFileAssetWarrantyCommandHandler> _logger;
        private readonly IIPAddressService _ipAddressService;
        private readonly IAssetMasterGeneralQueryRepository _assetMasterGeneralRepository;
        private readonly IAssetWarrantyQueryRepository _assetWarrantQueryRepository;
         private readonly IUnitGrpcClient _unitGrpcClient;
        private readonly ICompanyGrpcClient _companyGrpcClient;

        public DeleteFileAssetWarrantyCommandHandler(
            IFileUploadService fileUploadService,
            IAssetWarrantyCommandRepository assetWarrantyRepository,
            ILogger<DeleteFileAssetWarrantyCommandHandler> logger, IIPAddressService ipAddressService,IAssetMasterGeneralQueryRepository assetMasterGeneralRepository,IAssetWarrantyQueryRepository assetWarrantQueryRepository, IUnitGrpcClient unitGrpcClient, ICompanyGrpcClient companyGrpcClient)
        {
            _fileUploadService = fileUploadService;
            _assetWarrantyRepository = assetWarrantyRepository;
            _logger = logger; _ipAddressService = ipAddressService;_assetMasterGeneralRepository=assetMasterGeneralRepository;_assetWarrantQueryRepository=assetWarrantQueryRepository;
            _unitGrpcClient = unitGrpcClient;
            _companyGrpcClient = companyGrpcClient;
        }

       public async Task<ApiResponseDTO<bool>> Handle(DeleteFileAssetWarrantyCommand request, CancellationToken cancellationToken)
        {
            var companyId = _ipAddressService.GetCompanyId();
            var unitId = _ipAddressService.GetUnitId();
            var companies = await _companyGrpcClient.GetAllCompanyAsync();
            var units = await _unitGrpcClient.GetAllUnitAsync();

            var companyLookup = companies.ToDictionary(c => c.CompanyId, c => c.CompanyName);
            var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);

            var companyName = companyLookup.TryGetValue(companyId, out var cname) ? cname : string.Empty;
            var unitName = unitLookup.TryGetValue(unitId, out var uname) ? uname : string.Empty; 
            
            string baseDirectory = await _assetWarrantQueryRepository.GetBaseDirectoryAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                _logger.LogError("Base directory path not found in database.");
                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Base directory not configured." };                
            }
            
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory,companyName,unitName);       

            string filePath = Path.Combine(uploadPath, request.assetPath??string.Empty);

            var result = await _fileUploadService.DeleteFileAsync(filePath);

            await _assetWarrantyRepository.RemoveAssetWarrantyAsync(request.assetPath);
              if (result)
            {
                return new ApiResponseDTO<bool> { IsSuccess = true, Message = "File deleted successfully" };
            }
            return new ApiResponseDTO<bool> { IsSuccess = false, Message = "File deletion failed" };
        }

    }
}
