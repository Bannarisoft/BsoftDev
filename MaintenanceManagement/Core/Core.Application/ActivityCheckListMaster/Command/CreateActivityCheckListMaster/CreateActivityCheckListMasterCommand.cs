using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ActivityCheckListMaster.Command.CreateActivityCheckListMaster
{
    public class CreateActivityCheckListMasterCommand : IRequest<ApiResponseDTO<int>>
    {

        public int ActivityID { get; set; }
        public string? ActivityCheckList { get; set; }       
        public int  UnitId { get; set; }

    }
}