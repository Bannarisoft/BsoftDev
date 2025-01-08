using AutoMapper;
using Core.Application.Common.Interfaces.IUnit;
using MediatR;
using System.Data;

namespace Core.Application.Units.Queries.GetUnits
{
    public class GetUnitQueryHandler : IRequestHandler<GetUnitQuery,List<UnitDto>>
    {
        private readonly IUnitQueryRepository _unitRepository;        
        private readonly IMapper _mapper;
        public GetUnitQueryHandler( IUnitQueryRepository unitRepository, IMapper mapper)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
        }

        public async Task<List<UnitDto>> Handle(GetUnitQuery request, CancellationToken cancellationToken)
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

            var units = await _unitRepository.GetAllUnitsAsync();
            var unitList = _mapper.Map<List<UnitDto>>(units);
            return unitList;
        }
    }
}