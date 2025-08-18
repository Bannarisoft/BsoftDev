using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.RackMaster.Queries.GetAllRackMaster;
using MediatR;

namespace Core.Application.RackMaster.Queries.GetRackMasterById
{
    public class GetRackMasterByIdQuery : IRequest<RackMasterDto>
    {
        public int Id { get; set; }
        
    }
}