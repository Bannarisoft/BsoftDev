using Core.Application.Common.HttpResponse;
using Core.Application.Manufacture.Queries.GetManufacture;
using MediatR;

namespace Core.Application.Manufacture.Commands.CreateManufacture
{
    public class CreateManufactureCommand : IRequest<ApiResponseDTO<ManufactureDTO>>  
    {
        public string? Code { get; set; }        
        public string? Name { get; set; }                
        public string? Type { get; set; }
        public string? Country { get; set; }        
        public string? State { get; set; }        
        public string? City { get; set; }        
        public string? AddressLine1 { get; set; }        
        public string? AddressLine2 { get; set; }        
        public int? PinCode { get; set; }        
        public string? PersonName { get; set; }        
        public int? PhoneNumber { get; set; }        
        public string? Email { get; set; }   
    }
}