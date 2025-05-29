
using Core.Application.Common.Interfaces.ICompany;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.UserManagement;

namespace UserManagement.API.GrpcServices
{
    public class CompanyGrpcService : CompanyService.CompanyServiceBase
    {
        private readonly ICompanyQueryRepository _companyQueryRepository;
        public CompanyGrpcService(ICompanyQueryRepository companyQueryRepository)
        {
            _companyQueryRepository = companyQueryRepository;
        }
        public override async Task<CompanyListResponse> GetAllCompany(Empty request, ServerCallContext context)
        {
            var (companies, _) = await _companyQueryRepository.GetAllCompaniesAsync(1, int.MaxValue, null);

            var response = new CompanyListResponse();
            foreach (var company in companies)
            {
                response.Companies.Add(new CompanyDto
                {
                    CompanyId = company.Id,
                    CompanyName = company.CompanyName,
                    LegalName=company.LegalName,
                    GstNumber=company.GstNumber,
                    TinNumber=company.TIN,
                    TanNumber=company.TAN,
                    CstNumber=company.CSTNo,
                    EntityId=company.EntityId
                });
            }
            return response;
        }

    }
}