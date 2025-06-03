using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetMachineNoDepartmentbyId
{
    public class GetMachineNoDepartmentbyIdQuery : IRequest<ApiResponseDTO<List<GetMachineNoDepartmentbyIdDto>>>
    {
        public int DepartmentId { get; set; }
    }
}