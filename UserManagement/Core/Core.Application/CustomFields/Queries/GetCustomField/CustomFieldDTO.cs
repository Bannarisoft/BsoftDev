using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.CustomFields.Commands.CreateCustomField;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.CustomFields.Queries.GetCustomField
{
    public class CustomFieldDTO
    {
        public int Id { get; set; }
        public string LabelName { get; set; }
        public int Length { get; set; }
        public byte IsRequired { get; set; }
        public string LabelType { get; set; }
        public string DataType { get; set; }
        public Status IsActive { get; set; }
        
    }
}