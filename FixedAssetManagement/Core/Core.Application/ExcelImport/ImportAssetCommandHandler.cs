using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.Interfaces.IExcelImport;
using Core.Domain.Entities;
using MediatR;
using OfficeOpenXml;

namespace Core.Application.ExcelImport
{
    public class ImportAssetCommandHandler : IRequestHandler<ImportAssetCommand, bool>
    {
        private readonly IExcelImportCommandRepository _assetRepository;
        private readonly IExcelImportQueryRepository _assetQueryRepository;
        private readonly IMapper _mapper;

        public ImportAssetCommandHandler(IExcelImportCommandRepository assetRepository, IExcelImportQueryRepository assetQueryRepository, IMapper mapper)
        {
            _assetRepository = assetRepository;
            _assetQueryRepository = assetQueryRepository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(ImportAssetCommand request, CancellationToken cancellationToken)
        {
            if (request.ImportDto == null || request.ImportDto.File == null || request.ImportDto.File.Length == 0)
            {
                throw new ArgumentException("Invalid file uploaded.");
            }

            var assetsDto = new List<AssetMasterDto>();
            int currentRow = 0;

            using (var stream = new MemoryStream())
            {
                await request.ImportDto.File.CopyToAsync(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 3; row <= rowCount; row++)
                    {
                        try
                        {
                            currentRow = row;
                            var assetGroupHandler = new AssetGroupHandler(_assetRepository, _assetQueryRepository);
                            var assetDetailsHandler = new AssetDetailsHandler(_assetRepository, _assetQueryRepository);
                            var assetPurchaseHandler = new AssetPurchaseHandler(worksheet, row);
                            var assetInsuranceHandler = new AssetInsuranceHandler(worksheet, row);
                            var assetAdditionalCostHandler = new AssetAdditionalCostHandler(worksheet, row);
                            var assetSpecificationHandler = new AssetSpecificationHandler(worksheet, row);
                            var assetLocationHandler = new AssetLocationHandler(_assetRepository, _assetQueryRepository); // Pass the correct repositories                        

                            // Extracting all required details
                            var assetDto = await assetGroupHandler.ProcessAssetGroupAsync(request, worksheet, row);
                            await assetDetailsHandler.ProcessAssetDetailsAsync(request, worksheet, row, assetDto);
                            assetDto.AssetLocation = await assetLocationHandler.ProcessLocationAsync(worksheet, row);                         
                            assetDto.AssetPurchaseDetails = assetPurchaseHandler.ProcessAssetPurchase();
                            assetDto.AssetInsurance = assetInsuranceHandler.ProcessAssetInsurance();
                            assetDto.AssetAdditionalCost = assetAdditionalCostHandler.ProcessAssetAdditionalCost();
                            assetDto.AssetSpecification = assetSpecificationHandler.ProcessSpecifications();

                            assetsDto.Add(assetDto);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error at Excel Row {currentRow}: {ex.Message}");
                        }
                    }
                }
            }

            return await _assetRepository.ImportAssetsAsync(assetsDto, cancellationToken);
        }
    }
}
