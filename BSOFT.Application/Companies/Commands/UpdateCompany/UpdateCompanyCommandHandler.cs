using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;

namespace BSOFT.Application.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, int>
    {
        private readonly ICompanyRepository _icompanyRepository;
        private readonly ICompanyAddressRepository _icompanyAddressRepository;
        private readonly ICompanyContactRepository _icompanyContactRepository;
        private readonly IMapper _imapper;

        public UpdateCompanyCommandHandler(ICompanyRepository icompanyRepository, ICompanyAddressRepository icompanyAddressRepository, ICompanyContactRepository icompanyContactRepository, IMapper imapper)
        {
            _icompanyRepository = icompanyRepository;
            _icompanyAddressRepository = icompanyAddressRepository;
            _icompanyContactRepository = icompanyContactRepository;
            _imapper = imapper;
        }

          public async Task<int> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company  = _imapper.Map<Company>(request.Company);
               
                var companyaddress =     _imapper.Map<CompanyAddress>(request.CompanyAddresses);
                var companycontact =     _imapper.Map<CompanyContact>(request.CompanyContacts);
               var  CompanyId = await _icompanyRepository.UpdateAsync(request.Company.Id, company);
           

              if (CompanyId != null)
            {
               
                 Console.WriteLine("Update Hello Handler");
                await _icompanyAddressRepository.UpdateAsync(company.Id,companyaddress);
                
                await _icompanyContactRepository.UpdateAsync(company.Id,companycontact);
            }

            return 1;
        }
    }
}