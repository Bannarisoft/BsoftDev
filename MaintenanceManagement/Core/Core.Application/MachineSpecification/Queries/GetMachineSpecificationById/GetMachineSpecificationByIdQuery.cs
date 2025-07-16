using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MachineSpecification.Command;
using MediatR;

namespace Core.Application.MachineSpecification.Queries.GetMachineSpecificationById
{
    public class GetMachineSpecificationByIdQuery : IRequest<ApiResponseDTO<List<MachineSpecificationDto>>>
    {
        public int Id { get; set; }
    }
}