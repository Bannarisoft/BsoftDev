using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Queries.GetDetailSchedulerByDate
{
    public class GetDetailSchedulerByDateQuery : IRequest<ApiResponseDTO<List<DetailSchedulerByDateDto>>>
    {
        public DateOnly SchedulerDate { get; set; }
        public int DepartmentId { get; set; }
    }
}