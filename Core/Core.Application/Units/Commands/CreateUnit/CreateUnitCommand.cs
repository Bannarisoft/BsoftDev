using Core.Application.Common;
using Core.Application.Units.Queries.GetUnits;
using MediatR;


namespace Core.Application.Units.Commands.CreateUnit
{
    public class CreateUnitCommand : IRequest<Result<int>> 
    {
         public UnitDto UnitDto { get; set; }
         
    }



}