using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MaintenanceType.Queries.GetMaintenanceType;
using MediatR;

namespace Core.Application.MaintenanceType.Queries.GetMaintenanceTypeById
{
    public class GetMaintenanceTypeByIdQuery : IRequest<ApiResponseDTO<MaintenanceTypeDto>>
    {
        public int Id { get; set; }
    }
}