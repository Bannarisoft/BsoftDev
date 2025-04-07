using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Contracts.Models.Maintenance
{
    public class DepartmentDto
    {
        [JsonPropertyName("id")]
        public int DepartmentId { get; set; }
        [JsonPropertyName("deptName")]
        public string DepartmentName { get; set; }
    }
}