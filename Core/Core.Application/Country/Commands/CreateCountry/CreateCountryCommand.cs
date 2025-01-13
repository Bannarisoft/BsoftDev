     using Core.Application.Country.Queries.GetCountries;
     using MediatR;
     using Core.Application.Common;

     namespace Core.Application.Country.Commands.CreateCountry
     {     
          public class CreateCountryCommand :  IRequest<Result<CountryDto>>  
          {
               public string CountryCode { get; set; }=string.Empty;
               public string CountryName { get; set; }  =string.Empty;    
              // public int CreatedBy { get; set; }
               //public string? CreatedByName { get; set; } 
          }
          

     }