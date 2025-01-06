using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.ICompanyContact;
using Core.Application.Common.Interfaces.ICompanyAddress;
using Core.Application.Common.Interfaces.ICompany;

namespace Core.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, int>
    {
         private readonly ICompanyCommandRepository _icompanyRepository;
         private readonly ICompanyAddressCommandRepository _icompanyAddressRepository;
         private readonly ICompanyContactCommandRepository _icompanyContactRepository;
        private readonly IMapper _imapper;

        public CreateCompanyCommandHandler(ICompanyCommandRepository icompanyRepository,ICompanyAddressCommandRepository icompanyAddressRepository,ICompanyContactCommandRepository icompanyContactRepository, IMapper imapper)
        {
            _icompanyRepository = icompanyRepository;
            _icompanyAddressRepository = icompanyAddressRepository;
            _icompanyContactRepository = icompanyContactRepository;
            _imapper = imapper;
        }

        public async Task<int> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            
                var company  = _imapper.Map<Company>(request.Company);
               
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