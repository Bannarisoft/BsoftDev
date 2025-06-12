using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Power.Feeder.Queries.GetFeeder;
using MediatR;

namespace Core.Application.Power.Feeder.Queries.GetFeederById
{
    public class GetFeederByIdQuery : IRequest<ApiResponseDTO<GetFeederByIdDto>>
    {
        public int  Id { get; set; }        

    }
}