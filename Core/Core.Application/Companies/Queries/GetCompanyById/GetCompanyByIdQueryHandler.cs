using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Application.Companies.Queries.GetCompanies;
using System.Data;
using Core.Application.Common.Interfaces.ICompany;
using Core.Domain.Entities;

namespace Core.Application.Companies.Queries.GetCompanyById
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery,GetCompanyDTO>
    {
          private readonly ICompanyQueryRepository _companyRepository;
        private readonly IMapper _mapper;
         public GetCompanyByIdQueryHandler(ICompanyQueryRepository companyRepository, IMapper mapper)
        {
              _companyRepository = companyRepository;
             _mapper =mapper;
        } 
        public async Task<GetCompanyDTO> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
           
            var result = await _companyRepository.GetByIdAsync(request.CompanyId);
            return _mapper.Map<GetCompanyDTO>(result);
        }
    }
}