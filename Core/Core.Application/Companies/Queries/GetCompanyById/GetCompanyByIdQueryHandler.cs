using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Application.Companies.Queries.GetCompanies;

namespace Core.Application.Companies.Queries.GetCompanyById
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery,CompanyDTO>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

         public GetCompanyByIdQueryHandler(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository =companyRepository;
            _mapper =mapper;
        } 
        public async Task<CompanyDTO> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
           

          var company = await _companyRepository.GetByIdAsync(request.CompanyId);
          return _mapper.Map<CompanyDTO>(company);
        }
    }
}