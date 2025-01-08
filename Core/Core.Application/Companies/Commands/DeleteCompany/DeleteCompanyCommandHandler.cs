using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using Core.Application.Common.Interfaces.ICompany;

namespace Core.Application.Companies.Commands.DeleteCompany
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, int>
    {
        private readonly ICompanyCommandRepository _icompanyRepository;
        private readonly IMapper _imapper;
         public DeleteCompanyCommandHandler(ICompanyCommandRepository companyRepository ,IMapper imapper)
        {
            _icompanyRepository = companyRepository;
            _imapper = imapper;
        }

        public async Task<int> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var company  = _imapper.Map<Company>(request.CompanyDelete);
     
            return await _icompanyRepository.DeleteAsync(request.Id,company);
        }
    }
}