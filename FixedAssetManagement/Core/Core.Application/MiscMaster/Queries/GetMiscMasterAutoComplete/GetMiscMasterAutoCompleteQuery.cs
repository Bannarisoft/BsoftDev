using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.MiscMaster.Queries.GetMiscMasterAutoComplete
{
    public class GetMiscMasterAutoCompleteQuery  :  IRequest<ApiResponseDTO<List<GetMiscMasterAutoCompleteDto>>>
    {


          public string? MiscTypeCode { get; set; }
         public string? MiscTypeName { get; set; }
        
    }
}