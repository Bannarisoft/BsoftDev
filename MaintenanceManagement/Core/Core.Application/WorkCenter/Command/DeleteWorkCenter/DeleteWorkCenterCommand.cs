using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.WorkCenter.Command.DeleteWorkCenter
{
    public class DeleteWorkCenterCommand : IRequest<ApiResponseDTO<int>> 
    {
        public int Id { get; set; }
    }
}