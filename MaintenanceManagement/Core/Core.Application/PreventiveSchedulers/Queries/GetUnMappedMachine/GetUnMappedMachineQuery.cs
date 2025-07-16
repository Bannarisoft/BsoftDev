using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Queries.GetUnMappedMachine
{
    public class GetUnMappedMachineQuery : IRequest<ApiResponseDTO<List<UnMappedMachineDto>>>
    {
        public int Id { get; set; }
    }
}