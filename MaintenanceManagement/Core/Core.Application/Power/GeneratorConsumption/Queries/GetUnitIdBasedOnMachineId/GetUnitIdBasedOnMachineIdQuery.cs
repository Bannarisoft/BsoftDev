using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.Power.GeneratorConsumption.Queries.GetUnitIdBasedOnMachineId
{
    public class GetUnitIdBasedOnMachineIdQuery :  IRequest<List<GetMachineIdBasedonUnitDto>>
    {
       
    }
}