using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral
{
    public class CreateAssetMasterGeneralCommandHandler : IRequestHandler<CreateAssetMasterGeneralCommand, ApiResponseDTO<AssetMasterDto>>
    {
        private readonly IMapper _mapper;
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly IAssetMasterGeneralQueryRepository _assetMasterGeneralQueryRepository;
        private readonly IMediator _mediator;

        public CreateAssetMasterGeneralCommandHandler(IMapper mapper, IAssetMasterGeneralCommandRepository assetMasterGeneralRepository, IAssetMasterGeneralQueryRepository assetMasterGeneralQueryRepository,IMediator mediator)
        {
            _mapper = mapper;
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
            _assetMasterGeneralQueryRepository = assetMasterGeneralQueryRepository;
            _mediator = mediator;    
        } 

        public async Task<ApiResponseDTO<AssetMasterDto>> Handle(CreateAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {             
            // Get latest AssetCode
            var latestAssetCode = await _assetMasterGeneralQueryRepository.GetLatestAssetCode(request.AssetMaster.CompanyId,request.AssetMaster.UnitId, request.AssetMaster.AssetGroupId, request.AssetMaster.AssetCategoryId,request.AssetMaster.AssetLocation.DepartmentId,request.AssetMaster.AssetLocation.LocationId);
            var assetCode=latestAssetCode;
            var assetEntity  = _mapper.Map<AssetMasterGenerals>(request.AssetMaster);            
            assetEntity.AssetCode = assetCode; // Assign generated AssetCode       
            var result = await _assetMasterGeneralRepository.CreateAsync(assetEntity, cancellationToken);
            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetEntity.AssetCode ?? string.Empty,
                actionName: assetEntity.AssetName ?? string.Empty,
                details: $"AssetMasterGeneral '{assetEntity.AssetName}' was created. Code: {assetEntity.AssetCode}",
                module:"AssetMasterGeneral"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var assetMasterDTO = _mapper.Map<AssetMasterDto>(result);
            if (result.Id > 0)
            {

                  string tempFilePath = request.AssetMaster.AssetImage; // Path of the uploaded file with TEMP_ prefix

                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    string directory = Path.GetDirectoryName(tempFilePath) ?? string.Empty;
                    string newFileName = $"{result.AssetCode}{Path.GetExtension(tempFilePath)}"; // Rename as AssetCode
                    string newFilePath = Path.Combine(directory, newFileName);

                    try
                    {
                        File.Move(tempFilePath, newFilePath); // Rename the file
                        assetEntity.AssetImage = newFilePath.Replace(@"\", "/"); // Update the path in the database
                        await _assetMasterGeneralRepository.UpdateAsync(assetEntity); // Save changes to DB
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        Console.WriteLine($"Failed to rename file: {ex.Message}");
                    }
                }
                
                return new ApiResponseDTO<AssetMasterDto>{
                    IsSuccess = true, 
                    Message = "AssetMasterGeneral created successfully.",
                    Data = assetMasterDTO
                };
            }
            return  new ApiResponseDTO<AssetMasterDto>{
                IsSuccess = false, 
                Message = "AssetMasterGeneral not created."
            };      
        }
    }
}