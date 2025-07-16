using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MaintenanceType.Queries.GetMaintenanceType;
using MediatR;

namespace Core.Application.MaintenanceType.Queries.GetMaintenanceTypeAutoComplete
{
    public class GetMaintenanceTypeAutoCompleteQuery : IRequest<List<MaintenanceTypeAutoCompleteDto>>
    {
        public string? SearchPattern { get; set; }
    }
}