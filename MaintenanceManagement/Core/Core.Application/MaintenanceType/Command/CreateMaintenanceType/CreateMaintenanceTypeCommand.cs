using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MaintenanceType.Command.CreateMaintenanceType
{
    public class CreateMaintenanceTypeCommand :IRequest<int>
    {
         public string? TypeName { get; set; }
    }
}