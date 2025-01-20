using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using Core.Application.Common.Interfaces.ICompany;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Companies.Commands.DeleteCompany
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, ApiResponseDTO<bool>>
    {
        private readonly ICompanyCommandRepository _icompanyRepository;
        private readonly IMapper _imapper;
         public DeleteCompanyCommandHandler(ICompanyCommandRepository companyRepository ,IMapper imapper)
        {
            _icompanyRepository = companyRepository;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<bool>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var company  = _imapper.Map<Company>(request.CompanyDelete);
            var result = await _icompanyRepository.DeleteAsync(request.Id,company);
            if (result)
            {
                return new ApiResponseDTO<bool>{IsSuccess = true, Message = "Company deleted successfully"};
            }
            return new ApiResponseDTO<bool>{IsSuccess = false, Message = "Company not deleted"};
        }
    }
}