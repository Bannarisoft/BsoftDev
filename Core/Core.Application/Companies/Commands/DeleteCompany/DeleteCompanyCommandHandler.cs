using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;

namespace Core.Application.Companies.Commands.DeleteCompany
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, int>
    {
        private readonly ICompanyRepository _icompanyRepository;
        private readonly IMapper _imapper;
         public DeleteCompanyCommandHandler(ICompanyRepository companyRepository ,IMapper imapper)
        {
            _icompanyRepository = companyRepository;
            _imapper = imapper;
        }

        public async Task<int> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var company  = _imapper.Map<Company>(request.CompanyDelete);
            //  var Updatecompany = new Company()
            // {
            //     Id = request.Id,
            //     IsActive = request.IsActive 
            // };
            return await _icompanyRepository.DeleteAsync(request.Id,company);
        }
    }
}