using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.HSNMaster.Queries.GetAllHSNMaster;
using MediatR;

namespace Core.Application.HSNMaster.Queries.GetHSNMasterById
{
    public class GetHSNMasterByIdQuery : IRequest<ApiResponseDTO<HSNMasterDto>>
    {
        public int Id { get; set; }
    }
}