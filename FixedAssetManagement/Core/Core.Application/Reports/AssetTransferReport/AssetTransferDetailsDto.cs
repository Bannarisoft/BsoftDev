using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Reports.AssetTransferReport
{
    public class AssetTransferDetailsDto 
    {
        public int TransferId { get; set; }
        public DateTimeOffset DocDate { get; set; }
        public string? TransferTypeDesc { get; set; }
        public string? AssetCode { get; set; }
        public string? AssetName { get; set; }
        public int FromUnitId { get; set; }
        public int ToUnitId { get; set; }
        public int FromDepartmentId { get; set; }
        public int FromDepartmentName { get; set; }
        public int ToDepartmentId { get; set; }
        public int ToDepartmentName { get; set; }
        public string? FromCustodianName { get; set; }
        public string? ToCustodianName { get; set; }
        public string? Status { get; set; }
    }
}