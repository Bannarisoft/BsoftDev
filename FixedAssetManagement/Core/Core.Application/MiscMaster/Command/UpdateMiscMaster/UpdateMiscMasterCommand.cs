using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MiscMaster.Command.UpdateMiscMaster
{
    public class UpdateMiscMasterCommand : IRequest<ApiResponseDTO<bool>>
    {
        
         
        public int Id { get; set; }
        public int MiscTypeMasterId { get; set; }  
        public string? Code { get; set;}
        public string? Description { get; set;}
        public int SortOrder  { get; set;}
        public byte IsActive { get; set; }
    }
}