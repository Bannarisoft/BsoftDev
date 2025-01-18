using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.ICompany;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Companies.Queries.GetCompanies
{
    public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery,ApiResponseDTO<List<GetCompanyDTO>>>
    {  
        private readonly ICompanyQueryRepository _companyRepository;
        private readonly IMapper _mapper;
        public GetCompanyQueryHandler(ICompanyQueryRepository companyRepository, IMapper mapper)
        {
             _companyRepository = companyRepository;
             _mapper =mapper;
        } 
        public async Task<ApiResponseDTO<List<GetCompanyDTO>>> Handle(GetCompanyQuery requst, CancellationToken cancellationToken){
            
            var companies = await _companyRepository.GetAllCompaniesAsync();             
            
            var companylist = _mapper.Map<List<GetCompanyDTO>>(companies);
            
            var response =companylist.ToList();
            return new ApiResponseDTO<List<GetCompanyDTO>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = response
            };
        }
    }
}