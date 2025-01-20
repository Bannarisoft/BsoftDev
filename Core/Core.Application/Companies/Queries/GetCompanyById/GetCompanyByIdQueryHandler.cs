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
using Core.Application.Common.HttpResponse;

namespace Core.Application.Companies.Queries.GetCompanyById
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery,ApiResponseDTO<GetCompanyDTO>>
    {
          private readonly ICompanyQueryRepository _companyRepository;
        private readonly IMapper _mapper;
         public GetCompanyByIdQueryHandler(ICompanyQueryRepository companyRepository, IMapper mapper)
        {
              _companyRepository = companyRepository;
             _mapper =mapper;
        } 
        public async Task<ApiResponseDTO<GetCompanyDTO>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
           
            var result = await _companyRepository.GetByIdAsync(request.CompanyId);
            var company = _mapper.Map<GetCompanyDTO>(result);
            return new ApiResponseDTO<GetCompanyDTO> { IsSuccess = true, Message = "Success", Data = company };
        }
    }
}