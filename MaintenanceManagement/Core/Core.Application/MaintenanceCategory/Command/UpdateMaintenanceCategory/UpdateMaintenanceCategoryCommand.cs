using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MaintenanceCategory.Command.UpdateMaintenanceCategory
{
    public class UpdateMaintenanceCategoryCommand  :IRequest<ApiResponseDTO<int>>
    {
        public int Id {get;set;}
        public string? CategoryName { get; set; }
        public string? Description { get; set; }

        public byte IsActive { get; set; }
    }
}