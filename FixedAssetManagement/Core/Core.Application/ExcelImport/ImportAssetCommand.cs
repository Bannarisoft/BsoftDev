using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.ExcelImport
{
   public class ImportAssetCommand : IRequest<ApiResponseDTO<bool>>
    {
        public ImportAssetDto ImportDto { get; set; }

        public ImportAssetCommand(ImportAssetDto importDto)
        {
            ImportDto = importDto;
        }
    }
}