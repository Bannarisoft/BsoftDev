using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.ICompany;

namespace Core.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, int>
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

        public async Task<int> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            
             var company  = _imapper.Map<Company>(request.Company);
             string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/AllFiles");
             var uploadResult = await _ifileUploadService.UploadFileAsync(request.Company.File,  uploadPath);
             if (!uploadResult.IsSuccess)
             {
                 return 0;
             }
             company.Logo =uploadResult.FilePath;
            
             var CompanyId = await _icompanyRepository.CreateAsync(company);

                return CompanyId;
        }
    }
}