using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.CustomFields.Commands.DeleteCustomField
{
    public class DeleteCustomFieldCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
    }
}