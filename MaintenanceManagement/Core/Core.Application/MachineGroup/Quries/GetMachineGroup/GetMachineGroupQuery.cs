using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.MachineGroup.Quries.GetMachineGroup
{
    public class GetMachineGroupQuery :IRequest<ApiResponseDTO<List<MachineGroupDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? SearchTerm { get; set; }


    }
}