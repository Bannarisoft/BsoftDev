using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.PreventiveSchedulers.Commands.RescheduleBulkImport
{
    public class RescheduleBulkImportCommand : IRequest<ApiResponseDTO<string>>
    {
        public IFormFile File { get; set; }
    }
}