using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MachineMaster.Queries.GetMachineMaster;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetMachineMasterById
{
    public class GetMachineMasterByIdQuery : IRequest<ApiResponseDTO<MachineMasterDto>>
    {
        public int Id { get; set; }
    }
}