
using Core.Application.Common.HttpResponse;
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;
using MediatR;

namespace Core.Application.DepreciationDetail.Queries.GetDepreciationAbstract
{
    public class GetDepreciationAbstractQuery  : IRequest<ApiResponseDTO<List<DepreciationAbstractDto>>>
    {
        public int companyId {get; set;   }
        public int unitId {get; set;   }
        public string? finYear {get; set;}
        public int depreciationType {get; set;}
        public int depreciationPeriod { get; set; }
    }
}