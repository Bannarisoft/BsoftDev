using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Commands.MapMachine
{
    public class MapMachineCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public DateOnly LastMaintenanceActivityDate { get; set; }
    }
}