using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;
using AutoMapper;

namespace BSOFT.Application.Companies.Commands.DeleteCompany
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, int>
    {
        private readonly ICompanyRepository _companyRepository;
         public DeleteCompanyCommandHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }
        public async Task<int> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
             var Updatecompany = new Company()
            {
                CoId = request.Id,
                IsActive = request.IsActive 
            };
            return await _companyRepository.DeleteAsync(request.Id,Updatecompany);
        }
    }
}