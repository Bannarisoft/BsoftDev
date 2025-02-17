using Core.Application.Common.HttpResponse;
using Core.Application.Manufacture.Queries.GetManufacture;
using MediatR;

namespace Core.Application.Manufacture.Commands.CreateManufacture
{
    public class CreateManufactureCommand : IRequest<ApiResponseDTO<ManufactureDTO>>  
    {
        public string? Code { get; set; }        
        public string? ManufactureName { get; set; }                
        public string? ManufactureType { get; set; }
        public int CountryId { get; set; }        
        public int StateId { get; set; }        
        public int CityId { get; set; }        
        public string? AddressLine1 { get; set; }        
        public string? AddressLine2 { get; set; }        
        public string? PinCode { get; set; }        
        public string? PersonName { get; set; }        
        public string? PhoneNumber { get; set; }        
        public string? Email { get; set; }   
    }
}