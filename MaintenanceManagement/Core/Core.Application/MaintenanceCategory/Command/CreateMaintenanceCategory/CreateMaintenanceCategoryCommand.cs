using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MaintenanceCategory.Command.CreateMaintenanceCategory
{
    public class CreateMaintenanceCategoryCommand :IRequest<int>
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
    }
}