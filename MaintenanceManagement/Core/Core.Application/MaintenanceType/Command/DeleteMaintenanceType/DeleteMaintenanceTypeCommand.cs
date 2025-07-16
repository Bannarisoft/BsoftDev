using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MaintenanceType.Command.DeleteMaintenanceType
{
    public class DeleteMaintenanceTypeCommand : IRequest<int>
    {
        public int Id { get; set; } 
    }
}