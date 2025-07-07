using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MRS.Command.CreateMRS
{
    public class CreateMRSCommand :IRequest<int>
    {
         public HeaderRequest? Header { get; set; }
    }
}