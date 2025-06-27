using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ExcelImport.PhysicalStockVerification
{
    public class ImportAssetAuditCommand  : IRequest<ApiResponseDTO<bool>>
    {
        public ImportAssetAuditDto ImportAssetAuditDto { get; set; }

        public ImportAssetAuditCommand(ImportAssetAuditDto importAssetAuditDto)
        {
            ImportAssetAuditDto = importAssetAuditDto;
        }
    }
}