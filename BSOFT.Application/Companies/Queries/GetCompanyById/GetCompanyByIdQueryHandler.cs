using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BSOFT.Domain.Interfaces;
using MediatR;
using System.Text;
using BSOFT.Application.Companies.Queries.GetCompanies;

namespace BSOFT.Application.Companies.Queries.GetCompanyById
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery,CompanyVm>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

         public GetCompanyByIdQueryHandler(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository =companyRepository;
            _mapper =mapper;
        } 
        public async Task<CompanyVm> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
           

          var company = await _companyRepository.GetByIdAsync(request.CompanyId);
          return _mapper.Map<CompanyVm>(company);
        }
    }
}