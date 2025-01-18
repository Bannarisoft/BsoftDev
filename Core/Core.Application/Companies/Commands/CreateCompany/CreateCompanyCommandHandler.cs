using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.ICompany;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, ApiResponseDTO<int>>
    {
         private readonly ICompanyCommandRepository _icompanyRepository;
         private readonly IFileUploadService _ifileUploadService;
        private readonly IMapper _imapper;

        public CreateCompanyCommandHandler(ICompanyCommandRepository icompanyRepository, IMapper imapper, IFileUploadService ifileUploadService)
        {
            _icompanyRepository = icompanyRepository;
            _imapper = imapper;
            _ifileUploadService = ifileUploadService;
        }

        public async Task<ApiResponseDTO<int>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            
             var company  = _imapper.Map<Company>(request.Company);
             string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/AllFiles");
             var uploadResult = await _ifileUploadService.UploadFileAsync(request.Company.File,  uploadPath);
             if (!uploadResult.IsSuccess)
             {
                 return new ApiResponseDTO<int>{IsSuccess = false, Message = "File not uploaded"};
             }
             company.Logo =uploadResult.FilePath;
            
             var CompanyId = await _icompanyRepository.CreateAsync(company);

                if (CompanyId > 0)
                {
                    return new ApiResponseDTO<int>{IsSuccess = true, Message = "Division created successfully", Data = CompanyId};
                }
                return new ApiResponseDTO<int>{IsSuccess = false, Message = "Division not created"};
        }
    }
}