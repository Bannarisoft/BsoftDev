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
            // Define the base directory outside the project root
            string baseDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "AssetImages");            
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }            
            string companyFolder = Path.Combine(baseDirectory, request.CompanyName??string.Empty);
            if (!Directory.Exists(companyFolder))
            {
                Directory.CreateDirectory(companyFolder);
            }
            string unitFolder = Path.Combine(companyFolder, request.UnitName??string.Empty);            
            if (!Directory.Exists(unitFolder))
            {
                Directory.CreateDirectory(unitFolder);
            }

          /*   string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/AllFiles");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            } */
             var uploadResult = await _iFileUploadService.UploadFileAsync(request.File ,  unitFolder);
             if (!uploadResult.IsSuccess)
             {
                 return new ApiResponseDTO<AssetMasterGeneralDTO>{IsSuccess = false, Message = "File not uploaded"};
             }
            var response = new AssetMasterGeneralDTO
            {
                AssetImage = uploadResult.FilePath,
                AssetImageBase64 = uploadResult.logoBase64
            };            
            return new ApiResponseDTO<AssetMasterGeneralDTO>{IsSuccess = true, Data =  response};
        }
    }
}