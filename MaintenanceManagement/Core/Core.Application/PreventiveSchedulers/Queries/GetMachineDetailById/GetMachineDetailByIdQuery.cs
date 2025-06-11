using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Queries.GetMachineDetailById
{
    public class GetMachineDetailByIdQuery : IRequest<ApiResponseDTO<PreventiveSchedulerDto>>
    {
        public int Id { get; set; }
    }
}