using Core.Application.Units.Queries.GetUnits;
using MediatR;

namespace Core.Application.Units.Commands.DeleteUnit
{
    public class DeleteUnitCommand : IRequest<int>
    {
     public int UnitId { get; set; }
     public UnitStatusDto UpdateUnitStatusDto { get; set; }

    }
 
    }
    
