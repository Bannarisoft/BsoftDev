using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;

namespace BSOFT.Application.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, int>
    {
        private readonly ICompanyRepository _companyRepository;

        public UpdateCompanyCommandHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

          public async Task<int> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var UpdatecompanyEntity = new Company()
            {
                Id = request.Id,
                CompanyName = request.CompanyName,
                LegalName = request.LegalName,
                Address1 = request.Address1,
                Address2 = request.Address2,
                Address3 = request.Address3,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                GstNumber = request.GstNumber,
                TIN = request.TIN,
                TAN = request.TAN,
                CSTNo = request.CSTNo,   
                YearofEstablishment = request.YearofEstablishment,
                Website = request.Website,
                Logo = request.Logo,
                Entity = request.Entity,
                IsActive = request.IsActive,
                ModifiedBy = request.ModifiedBy, 
                Modified_Time = request.Modified_Time,
                ModifiedByName = request.ModifiedByName  
            };

            return await _companyRepository.UpdateAsync(request.Id, UpdatecompanyEntity);
        }
    }
}