using Core.Application.Common.HttpResponse;
using Core.Application.Companies.Queries.GetCompanies;
using MediatR;

namespace Core.Application.Companies.Queries.GetCompanyAutoComplete
{
    public class GetCompanyAutoCompleteQuery : IRequest<ApiResponseDTO<List<CompanyAutoCompleteDTO>>>
    {
        
        public string? SearchPattern { get; set; }
    }
}