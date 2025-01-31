using Core.Application.Common.Mappings;
using Core.Domain.Entities;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.State.Queries.GetStates
{
    public class StateDto  : IMapFrom<States>
    {
        public int Id { get; set; }
        public string? StateCode { get; set; } 
        public string? StateName { get; set; } 
        public int CountryId { get; set; }   
         public Status IsActive { get; set; }          
    }
}