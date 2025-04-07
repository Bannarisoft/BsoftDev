using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.ActivityCheckListMaster.Command.UpdateActivityCheckListMaster
{
    public class UpdateActivityCheckListMasterCommand : IRequest<ApiResponseDTO<int>>
    {
       public int Id { get; set; }
       public int ActivityID { get; set; }       
       public string? ActivityChecklist { get; set; }

       public byte IsActive { get; set; }
        
    }
}