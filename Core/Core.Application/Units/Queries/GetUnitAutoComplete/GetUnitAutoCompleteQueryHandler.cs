using MediatR;
using Core.Application.Units.Queries.GetUnits;
using System.Data;
using Core.Application.Common.Interfaces.IUnit;
using AutoMapper;
using Core.Application.Common;
using Core.Domain.Events;
using Core.Application.Common.Exceptions;

namespace Core.Application.Units.Queries.GetUnitAutoComplete
{
    public class GetUnitAutoCompleteQueryHandler : IRequestHandler<GetUnitAutoCompleteQuery, Result<List<UnitDto>>>
    {
         private readonly IUnitQueryRepository _unitRepository;        
        private readonly IMapper _mapper;

        private readonly IMediator _mediator; 


        public GetUnitAutoCompleteQueryHandler(IUnitQueryRepository unitRepository, IMapper mapper, IMediator mediator)
        {
             _unitRepository = unitRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<Result<List<UnitDto>>> Handle(GetUnitAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            /*  var query = @"
                  SELECT 
                    u.Id,
                    u.UnitName,
                    u.ShortName,
                    u.CompanyId,
                    u.DivisionId,
                    u.UnitHeadName,
                    u.CINNO,
                    u.IsActive,
                    ua.Id as AddressId,
                    ua.UnitId,
                    ua.CountryId,
                    ua.StateId,
                    ua.CityId,
                    ua.AddressLine1,
                    ua.AddressLine2,
                    ua.PinCode,
                    ua.ContactNumber,
                    ua.AlternateNumber,
                    uc.Id as ContactId,
                    uc.UnitId,
                    uc.Name,
                    uc.Designation,
                    uc.Email,
                    uc.PhoneNo,
                    uc.Remarks                   
                FROM 
                    AppData.Unit  u
                INNER JOIN 
                    AppData.UnitAddress  ua ON u.Id = ua.UnitId
                INNER JOIN 
                    AppData.UnitContacts uc ON u.Id = uc.UnitId
				WHERE U.UnitName LIKE @SearchPattern OR U.Id LIKE @SearchPattern
                AND U.IsActive = 1
                ORDER BY U.UnitName";
             var result = await _dbConnection.QueryAsync<UnitDto, UnitAddressDto, UnitContactsDto, UnitDto>(
                query,
                (unit, address, contact) =>
                {
                    unit.UnitAddressDto.Add(address);
                    unit.UnitContactsDto.Add(contact);
                    return unit;
                },
                splitOn: "AddressId, ContactId",
                param: new { SearchPattern = $"%{request.SearchPattern}%" });
                var units = result.GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList(); 
                return units;    */       
           
            var result = await _unitRepository.GetUnit(request.SearchPattern);
              if (result == null || !result.Any())
                {
                return Result<List<UnitDto>>.Failure("Unit not found.");
                }

            var unitDto = _mapper.Map<List<UnitDto>>(result);

            //Domain Event            
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetUnitAutoCompleteQuery",
                actionCode:"",        
                actionName: request.SearchPattern,                
                details: $"Unit '{request.SearchPattern}' was searched",
                module:"Unit"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return Result<List<UnitDto>>.Success(unitDto);                                    
        }
    }
}