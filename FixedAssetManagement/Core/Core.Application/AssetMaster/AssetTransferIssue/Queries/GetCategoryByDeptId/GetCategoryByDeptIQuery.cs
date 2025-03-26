using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetCategoryByDeptId
{
    public class GetCategoryByDeptIQuery  :  IRequest<ApiResponseDTO<List<GetCategoryByDeptIdDto>>>
    {    

        public int DepartmentId { get; set; }

        
    }
}