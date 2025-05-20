using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.DepartmentGroup.Command.CreateDepartmentGroup
{
    public class CreateDepartmentGroupCommand : IRequest<ApiResponseDTO<int>>  // Return the created ID
    {   
        public string? DepartmentGroupCode { get; set; }
        public string? DepartmentGroupName { get; set; }       
        public byte IsActive { get; set; }
  
    }
}