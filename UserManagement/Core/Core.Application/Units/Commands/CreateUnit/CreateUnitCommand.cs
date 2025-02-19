using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Units.Queries.GetUnits;
using MediatR;


namespace Core.Application.Units.Commands.CreateUnit
{
    public class CreateUnitCommand : IRequest<ApiResponseDTO<int>> 
    {
    public string? UnitName { get; set; }
    public string? ShortName { get; set; }
    public int CompanyId { get; set; }
    public int DivisionId { get; set; }
    public string? UnitHeadName { get; set; }
    public string? CINNO { get; set; }
    public string? OldUnitId { get; set; }
    public UnitAddressDto? UnitAddressDto { get; set; } 
    public UnitContactsDto? UnitContactsDto { get; set;}   
    }



}