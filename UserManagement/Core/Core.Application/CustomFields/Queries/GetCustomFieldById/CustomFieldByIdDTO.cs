using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.CustomFields.Queries.GetCustomFieldById
{
    public class CustomFieldByIdDTO
    {
         public int Id { get; set; }
        public string LabelName { get; set; }
        public int Length { get; set; }
        public byte IsRequired { get; set; }
        public int LabelTypeId { get; set; }
        public int DataTypeId { get; set; }
        public Status IsActive { get; set; }
    }
}