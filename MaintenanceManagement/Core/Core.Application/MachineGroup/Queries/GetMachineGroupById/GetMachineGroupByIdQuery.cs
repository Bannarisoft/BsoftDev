using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineGroup.Queries.GetMachineGroupById
{
    public class GetMachineGroupByIdQuery   :  IRequest<ApiResponseDTO<GetMachineGroupByIdDto>>
    {
        public int Id { get; set; }
    }
}