using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Queries.GetPreventiveSchedulerById
{
    public class GetPreventiveSchedulerByIdQuery : IRequest<ApiResponseDTO<PreventiveSchedulerHdrByIdDto>>
    {
        public int Id { get; set; }
    }
}