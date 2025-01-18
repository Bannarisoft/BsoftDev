using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Units.Queries.GetUnits;
using MediatR;


namespace Core.Application.Units.Commands.CreateUnit
{
    public class CreateUnitCommand : IRequest<ApiResponseDTO<int>> 
    {
         public UnitDto UnitDto { get; set; }
         
    }



}