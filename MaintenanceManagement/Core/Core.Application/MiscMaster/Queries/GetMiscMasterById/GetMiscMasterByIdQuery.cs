using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.MiscMaster.Queries.GetMiscMasterById
{
    public class GetMiscMasterByIdQuery  :  IRequest<ApiResponseDTO<GetMiscMasterDto>>
    { 
         public int Id { get; set; }
    }
}