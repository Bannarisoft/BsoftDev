using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Power.PowerConsumption.Queries.GetClosingReaderValueById
{
    public class GetClosingReaderValueByIdQuery :  IRequest<GetClosingReaderValueDto>
    {
        public int FeederId { get; set; }
    }
}