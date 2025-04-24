using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MaintenanceRequest.Command.UpdateMaintenanceRequestStatusCommand
{
    public class UpdateMaintenanceRequestStatusCommand  : IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
       // public int RequestStatusId { get; set; }
    }
}