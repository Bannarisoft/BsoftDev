using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MaintenanceType.Command.UpdateMaintenanceType
{
    public class UpdateMaintenanceTypeCommand :IRequest<int>
    {
        public int Id {get;set;}
        public string? TypeName { get; set; }
        public byte IsActive { get; set; }
    }
}