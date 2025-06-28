using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetCategoryByCustodian
{
    public class GetCategoryByCustodianQuery : IRequest<ApiResponseDTO<List<GetCategoryByCustodianDto>>>
    {
          public int DepartmentId { get; set; }
        public string? CustodianId { get; set; }      
        
    }
}