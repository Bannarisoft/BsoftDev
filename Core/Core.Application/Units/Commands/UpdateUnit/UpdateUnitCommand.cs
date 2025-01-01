using Core.Application.Units.Queries.GetUnits;
using MediatR;

namespace Core.Application.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommand : IRequest<int>
    {    
    public int UnitId  { get; set; }
    public UnitDto UpdateUnitDto { get; set; }  
    }
}