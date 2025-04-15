using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;
using MediatR;

namespace Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequestById
{
    public class GetMaintenanceRequestByIdQuery  :  IRequest<ApiResponseDTO<GetMaintenanceRequestDto>>
    {
        public int Id { get; set; }
    }
}