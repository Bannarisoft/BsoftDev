using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Power.FeederGroup.Command.UpdateFeederGroup
{
    public class UpdateFeederGroupCommand  : IRequest<ApiResponseDTO<bool>>
    { 
        public int Id { get; set; }
        public string? FeederGroupCode { get; set; }
        public string? FeederGroupName { get; set; }        
        public byte IsActive { get; set; }
    }
}