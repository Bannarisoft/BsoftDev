
using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral
{
    public class UpdateAssetMasterGeneralCommandHandler : IRequestHandler<UpdateAssetMasterGeneralCommand, ApiResponseDTO<bool>>
    {
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly IAssetMasterGeneralQueryRepository _assetMasterGeneralQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public UpdateAssetMasterGeneralCommandHandler(IAssetMasterGeneralCommandRepository assetMasterGeneralRepository, IMapper mapper,IAssetMasterGeneralQueryRepository assetMasterGeneralQueryRepository, IMediator mediator)
        {
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
            _mapper = mapper;
            _assetMasterGeneralQueryRepository = assetMasterGeneralQueryRepository;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<bool>> Handle(UpdateAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {
            var assetMaster = await _assetMasterGeneralQueryRepository.GetByIdAsync(request.AssetMaster.Id);
            if (assetMaster is null)
            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "Invalid AssetId. The specified AssetName does not exist or is inactive."
            };
            var oldAssetName = assetMaster.AssetName;
            assetMaster.AssetName = request.AssetMaster.AssetName;

         
            var updatedAssetMasterEntity = _mapper.Map<AssetMasterGenerals>(request.AssetMaster);                   
            var updateResult = await _assetMasterGeneralRepository.UpdateAsync( request.AssetMaster.Id,updatedAssetMasterEntity);            

          
                
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: request.AssetMaster.AssetCode ?? string.Empty,
                    actionName: request.AssetMaster.AssetName ?? string.Empty,                            
                    details: $"AssetMaster '{oldAssetName}' was updated to '{request.AssetMaster.AssetName}'.  Code: {request.AssetMaster.AssetCode}",
                    module:"AssetMasterGeneral"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult)
                {
                    string tempFilePath = request.AssetMaster.AssetImage;
                    string tempDocumentPath = request.AssetMaster.AssetDocument;
                    if (tempFilePath != null){
                        
                        string baseDirectory = await _assetMasterGeneralQueryRepository.GetBaseDirectoryAsync();
                        var (companyName, unitName) = await _assetMasterGeneralQueryRepository.GetCompanyUnitAsync(request.AssetMaster.CompanyId, request.AssetMaster.UnitId);
                        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory,companyName,unitName);     
                        string filePath = Path.Combine(uploadPath, tempFilePath);  
                        EnsureDirectoryExists(Path.GetDirectoryName(filePath));           

                        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                        {
                            string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
                            string newFileName = $"{request.AssetMaster.AssetCode}{Path.GetExtension(tempFilePath)}";
                            string newFilePath = Path.Combine(directory, newFileName);

                            try
                            {
                                File.Move(filePath, newFilePath);
                                //assetEntity.AssetImage = newFileName;
                                await _assetMasterGeneralRepository.UpdateAssetImageAsync(request.AssetMaster.Id, newFileName);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to rename file: {ex.Message}");
                            }
                        }
                    }  
                    //Document
                    if (tempDocumentPath != null){
                        string baseDirectory = await _assetMasterGeneralQueryRepository.GetDocumentDirectoryAsync();
                        var (companyName, unitName) = await _assetMasterGeneralQueryRepository.GetCompanyUnitAsync(request.AssetMaster.CompanyId, request.AssetMaster.UnitId);
                        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory,companyName,unitName);     
                        string filePath = Path.Combine(uploadPath, tempFilePath);  
                        EnsureDirectoryExists(Path.GetDirectoryName(filePath));           

                        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                        {
                            string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
                            string newFileName = $"{request.AssetMaster.AssetCode}{Path.GetExtension(tempFilePath)}";
                            string newFilePath = Path.Combine(directory, newFileName);
                            try
                            {
                                File.Move(filePath, newFilePath);
                                //assetEntity.AssetImage = newFileName;
                                await _assetMasterGeneralRepository.UpdateAssetDocumentAsync(request.AssetMaster.Id, newFileName);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to rename file: {ex.Message}");
                            }
                        }
                    }       
                    
                    return new ApiResponseDTO<bool>
                    {
                        IsSuccess = true,
                        Message = "AssetMaster updated successfully."                        
                    };
                }
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "AssetMaster not updated."
                };                          
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