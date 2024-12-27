using Core.Application.Companies.Queries.GetCompanies;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace Core.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, int>
    {
         private readonly ICompanyRepository _icompanyRepository;
         private readonly ICompanyAddressRepository _icompanyAddressRepository;
         private readonly ICompanyContactRepository _icompanyContactRepository;
        private readonly IMapper _imapper;

        public CreateCompanyCommandHandler(ICompanyRepository icompanyRepository,ICompanyAddressRepository icompanyAddressRepository,ICompanyContactRepository icompanyContactRepository, IMapper imapper)
        {
            _icompanyRepository = icompanyRepository;
            _icompanyAddressRepository = icompanyAddressRepository;
            _icompanyContactRepository = icompanyContactRepository;
            _imapper = imapper;
        }

        public async Task<int> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            
                var company  = _imapper.Map<Company>(request);
               
                var companyaddress =     _imapper.Map<CompanyAddress>(request.CompanyAddresses);
                var companycontact =     _imapper.Map<CompanyContact>(request.CompanyContacts);
             
         
            var CompanyId = await _icompanyRepository.CreateAsync(company);
                
            if (CompanyId != null)
            {
                
                 companyaddress.CompanyId = CompanyId.Id;
                 companycontact.CompanyId = CompanyId.Id;
                 
                await _icompanyAddressRepository.CreateAsync(companyaddress);
                
                await _icompanyContactRepository.CreateAsync(companycontact);
            }

            return CompanyId.Id;
        }
    }
}