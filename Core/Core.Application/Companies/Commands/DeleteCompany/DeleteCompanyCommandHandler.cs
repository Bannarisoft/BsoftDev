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
        private readonly ICompanyRepository _companyRepository;
         public DeleteCompanyCommandHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }
        public async Task<int> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
             var Updatecompany = new Company()
            {
                Id = request.Id,
                IsActive = request.IsActive 
            };
            return await _companyRepository.DeleteAsync(request.Id,Updatecompany);
        }
    }
}