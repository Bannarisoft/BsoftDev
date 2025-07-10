using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineSpecification.Command.UpdateMachineSpecfication
{
    public class UpdateMachineSpecficationCommand : IRequest<ApiResponseDTO<bool>>
    {
        public List<MachineSpecificationUpdateDto>? Specifications { get; set; }
    }
    public class MachineSpecificationUpdateDto
    {
        public int SpecificationId { get; set; }
        public string? SpecificationValue { get; set; }
        public int MachineId { get; set; }
    
    }
}