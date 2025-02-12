using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using MediatR;

namespace Core.Application.DepreciationGroup.Queries.GetDepreciationGroupById
{
    public class GetDepreciationGroupByIdQuery : IRequest<ApiResponseDTO<DepreciationGroupDTO>>
    {
        public int Id { get; set; }
    }
}