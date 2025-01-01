using Core.Application.Units.Queries.GetUnits;
using MediatR;


namespace Core.Application.Units.Commands.CreateUnit
{
    public class CreateUnitCommand : IRequest<int>
    {
         public UnitDto UnitDto { get; set; }
    }



}