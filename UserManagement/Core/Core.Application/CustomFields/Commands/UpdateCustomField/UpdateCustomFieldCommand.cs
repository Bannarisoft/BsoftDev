using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.CustomFields.Commands.UpdateCustomField
{
    public class UpdateCustomFieldCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
        public string LabelName { get; set; }
        public int Length { get; set; }
        public int DataTypeId { get; set; }
        public int LabelTypeId { get; set; }
        public byte IsRequired { get; set; }
        public byte IsActive { get; set; }

    }
}