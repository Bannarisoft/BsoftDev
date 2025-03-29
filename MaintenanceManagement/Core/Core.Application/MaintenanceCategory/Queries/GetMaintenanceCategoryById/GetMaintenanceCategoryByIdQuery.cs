using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MaintenanceCategory.Queries.GetMaintenanceCategory;
using MediatR;

namespace Core.Application.MaintenanceCategory.Queries.GetMaintenanceCategoryById
{
    public class GetMaintenanceCategoryByIdQuery : IRequest<ApiResponseDTO<MaintenanceCategoryDto>>
    {
        public int Id { get; set; }
    }
}