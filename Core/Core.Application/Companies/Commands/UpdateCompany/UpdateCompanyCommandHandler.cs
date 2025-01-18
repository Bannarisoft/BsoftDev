using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICompany;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, bool>
    {
        private readonly ICompanyCommandRepository _icompanyRepository;
        private readonly IFileUploadService _ifileUploadService;
        private readonly IMapper _imapper;
        private readonly ICompanyQueryRepository _companyQueryRepository;

        public UpdateCompanyCommandHandler(ICompanyCommandRepository icompanyRepository, IMapper imapper, IFileUploadService ifileUploadService, ICompanyQueryRepository companyQueryRepository)
        {
            _icompanyRepository = icompanyRepository;
            _imapper = imapper;
            _ifileUploadService = ifileUploadService;
            _companyQueryRepository = companyQueryRepository;
        }

          public async Task<bool> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine(request.Company.Id);
            var existingCompany = await _companyQueryRepository.GetByIdAsync(request.Company.Id);
            Console.WriteLine(existingCompany.Logo);
            var company  = _imapper.Map<Company>(request.Company);
            Console.WriteLine(request.Company.LogoPath);
            if(existingCompany.Logo == request.Company.LogoPath)
            {
                company.Logo = existingCompany.Logo;
            }
            else
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/AllFiles");
             var uploadResult = await _ifileUploadService.UploadFileAsync(request.Company.File,  uploadPath);
             
                if (!uploadResult.IsSuccess)
                 {
                     return false;
                 }
                 company.Logo =uploadResult.FilePath;
            }

             var  CompanyId = await _icompanyRepository.UpdateAsync(request.Company.Id, company);
           
            return CompanyId;
        }
    }
}