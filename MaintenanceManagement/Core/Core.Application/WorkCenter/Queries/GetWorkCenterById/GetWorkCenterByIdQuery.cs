using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.WorkCenter.Queries.GetWorkCenter;
using MediatR;

namespace Core.Application.WorkCenter.Queries.GetWorkCenterById
{
    public class GetWorkCenterByIdQuery : IRequest<ApiResponseDTO<WorkCenterDto>>
    {
         public int Id { get; set; }
    }
}