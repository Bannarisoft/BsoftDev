using BSOFT.Application.Companies.Queries.GetCompanies;
using BSOFT.Domain.Entities;
using BSOFT.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CompanyVm>
    {
         private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CreateCompanyCommandHandler(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<CompanyVm> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var companyEntity = new Company
            {
                CompanyName = request.CompanyName,
                LegalName = request.LegalName,
                Address1 = request.Address1,
                Address2 = request.Address2,
                Address3 = request.Address3,
                Phone = request.Phone,
                Email = request.Email,
                GstNumber = request.GstNumber,
                TIN = request.TIN,
                TAN = request.TAN,
                CSTNo = request.CSTNo,   
                YearofEstablishment = request.YearofEstablishment,
                Website = request.Website,
                Logo = request.Logo,
                EntityId = request.EntityId,
                IsActive = request.IsActive,
                CreatedBy = request.CreatedBy,
                CreatedAt = request.CreatedAt,
                CreatedByName = request.CreatedByName,
                CreatedIP =request.CreatedIP           
            };

            var result = await _companyRepository.CreateAsync(companyEntity);
            return _mapper.Map<CompanyVm>(result);
        }
    }
}