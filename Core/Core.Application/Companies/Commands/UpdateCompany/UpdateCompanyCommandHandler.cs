using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICompany;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, ApiResponseDTO<bool>>
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

          public async Task<ApiResponseDTO<bool>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            
            var existingCompany = await _companyQueryRepository.GetByIdAsync(request.Company.Id);
            
            var company  = _imapper.Map<Company>(request.Company);
            
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
                     return new ApiResponseDTO<bool>{IsSuccess = false, Message = "File not uploaded"};
                 }
                 company.Logo =uploadResult.FilePath;
            }

             var  CompanyId = await _icompanyRepository.UpdateAsync(request.Company.Id, company);
           
           if (CompanyId)
           {
               return new ApiResponseDTO<bool>{IsSuccess = true, Message = "Company updated successfully"};
           }
            return new ApiResponseDTO<bool>{IsSuccess = false, Message = "Company not updated"};
        }
    }
}