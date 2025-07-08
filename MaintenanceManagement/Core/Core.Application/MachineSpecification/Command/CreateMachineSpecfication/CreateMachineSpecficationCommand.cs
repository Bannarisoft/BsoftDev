using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineSpecification.Command.CreateMachineSpecfication
{
    public class CreateMachineSpecficationCommand : IRequest<ApiResponseDTO<List<int>>>
    {
    public List<MachineSpecificationCreateDto>? Specifications { get; set; }
    }
    public class MachineSpecificationCreateDto
    {
    public int SpecificationId { get; set; }
    public int MachineId { get; set; }
    }
}