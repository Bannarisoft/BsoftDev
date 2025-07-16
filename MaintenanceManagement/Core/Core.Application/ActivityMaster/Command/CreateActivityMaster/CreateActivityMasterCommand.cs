using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ActivityMaster.Queries.GetAllActivityMaster;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ActivityMaster.Command.CreateActivityMaster
{
    public class CreateActivityMasterCommand : IRequest<int>
    {

        public CreateActivityMasterDto? CreateActivityMasterDto { get; set; }
        


    }
}