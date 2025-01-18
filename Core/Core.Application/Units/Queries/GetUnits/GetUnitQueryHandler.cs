using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.IUnit;
using Core.Domain.Events;
using MediatR;
using System.Data;

namespace Core.Application.Units.Queries.GetUnits
{
    public class GetUnitQueryHandler : IRequestHandler<GetUnitQuery,Result<List<UnitDto>>>
    {
        private readonly IUnitQueryRepository _unitRepository;        
        private readonly IMapper _mapper;

        private readonly IMediator _mediator;
        public GetUnitQueryHandler( IUnitQueryRepository unitRepository, IMapper mapper, IMediator mediator)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<Result<List<UnitDto>>> Handle(GetUnitQuery request, CancellationToken cancellationToken)
        {
         /*    var query = @"
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
            ";

            var result = await _dbConnection.QueryAsync<UnitDto, UnitAddressDto, UnitContactsDto, UnitDto>(
            query,
            (unit, address, contact) =>
            {
                unit.UnitAddressDto.Add(address);
                unit.UnitContactsDto.Add(contact);
                return unit;
            },
            splitOn: "AddressId, ContactId");
            var units = result.GroupBy(u => u.Id)
            .Select(g => g.First())
            .ToList(); 
            return units; */
            try
            {
            var units = await _unitRepository.GetAllUnitsAsync();
            var unitList = _mapper.Map<List<UnitDto>>(units);
            //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetUnitQuery",
                    actionCode: "",        
                    actionName: "",
                    details: $"Units details was fetched.",
                    module:"Unit"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return Result<List<UnitDto>>.Success(unitList);
            } 
            catch (Exception ex)
            {
             // Throw a generic CustomException for unexpected errors
            throw new CustomException(
            "An unexpected error occurred while Fetching the Unit.",
            new[] { ex.Message },
            CustomException.HttpStatus.InternalServerError
            );
            }
        
        }
    }
}