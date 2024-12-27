using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;

namespace Core.Application.Companies.Queries.GetCompanies
{
    public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery,List<CompanyDTO>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        public GetCompanyQueryHandler(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository =companyRepository;
            _mapper =mapper;
        } 
        public async Task<List<CompanyDTO>> Handle(GetCompanyQuery requst, CancellationToken cancellationToken){

            var companies = await _companyRepository.GetAllCompaniesAsync();
            
            var companylist = _mapper.Map<List<CompanyDTO>>(companies);

            return companylist;
        }
    }
}