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


        public ImportAssetCommandHandler(IExcelImportCommandRepository assetRepository,IExcelImportQueryRepository assetQueryRepository ,IMapper mapper)
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
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 3; row <= rowCount; row++) // Assuming first row is header
                    {
                        try
                        {
                            currentRow = row;
                            //AssetGroup
                            string assetGroupName = worksheet.Cells[row, 2].Value?.ToString()  ?? string.Empty;                             
                            int? assetGroupId = await _assetRepository.GetAssetGroupIdByNameAsync(assetGroupName);
                            if (assetGroupId == null)
                            {
                                throw new Exception($"Invalid Asset Group Name '{assetGroupName}' at Excel Row {currentRow}");
                            }
                             //AssetCategory
                            string assetCategory = worksheet.Cells[row, 3].Value?.ToString()  ?? string.Empty;                             
                            int? assetCategoryId = await _assetRepository.GetAssetCategoryIdByNameAsync(assetCategory);
                            if (assetCategoryId == null)
                            {
                                throw new Exception($"Invalid Asset Category Name '{assetCategory}' at Excel Row {currentRow}");
                            }
                            //AssetSubcategory
                            string assetSubCategory = worksheet.Cells[row, 4].Value?.ToString() ?? string.Empty;                             
                            int? assetSubCategoryId = await _assetRepository.GetAssetSubCategoryIdByNameAsync(assetSubCategory);
                            if (assetSubCategoryId == null)
                            {
                                throw new Exception($"Invalid Asset Sub Category Name '{assetSubCategory}' at Excel Row {currentRow}");
                            }
                            //AssetUOM
                            string assetUOM = worksheet.Cells[row, 8].Value?.ToString() ?? string.Empty;                             
                            int? assetUOMId = await _assetRepository.GetAssetUOMIdByNameAsync(assetUOM);
                            if (assetUOMId == null)
                            {
                                throw new Exception($"Invalid Asset UOM '{assetUOM}' at Excel Row {currentRow}");
                            }
                             //Unit
                            string assetUnit = worksheet.Cells[row, 11].Value?.ToString() ?? string.Empty;                             
                            int? assetUnitId = await _assetQueryRepository.GetAssetUnitIdByNameAsync(assetUnit);
                            if (assetUnitId == null)
                            {
                                throw new Exception($"Invalid Asset Unit '{assetUnit}' at Excel Row {currentRow}");
                            }
                            //Dept
                            string assetDept = worksheet.Cells[row, 12].Value?.ToString() ?? string.Empty;                             
                            int? assetDeptId = await _assetQueryRepository.GetAssetDeptIdByNameAsync(assetDept);
                            if (assetDeptId == null)
                            {
                                throw new Exception($"Invalid Asset Department Name '{assetDept}' at Excel Row {currentRow}");
                            }
                            //AssetLocation
                            string assetLocation = worksheet.Cells[row, 13].Value?.ToString() ?? string.Empty;                             
                            int? assetLocationId = await _assetRepository.GetAssetLocationIdByNameAsync(assetLocation);
                           /*  if (assetLocationId == null)
                            {
                                throw new Exception($"Invalid Asset Location Name '{assetLocation}' at Excel Row {currentRow}");
                            } */
                            //AssetSubLocation
                            string assetSubLocation = worksheet.Cells[row, 14].Value?.ToString() ?? string.Empty;                             
                            int? assetSubLocationId = await _assetRepository.GetAssetSubLocationIdByNameAsync(assetSubLocation);
                            /* if (assetSubLocationId == null)
                            {
                                throw new Exception($"Invalid Asset SubLocation Name '{assetSubLocation}' at Excel Row {currentRow}");
                            } */
                               //Unit
                            int getUnit = request.ImportDto.UnitId;                             
                            UnitDto? unitDetails  = await _assetQueryRepository.GetUnitByNameAsync(getUnit);
                            if (unitDetails  == null)
                            {
                                throw new Exception($"Invalid Asset Unit Name '{getUnit}' at Excel Row {currentRow}");
                            }
                               //Company
                            int getCompany = request.ImportDto.CompanyId;
                            string? getCompanyId = await _assetQueryRepository.GetCompanyByNameAsync(getCompany);
                            if (getCompanyId == null)
                            {
                                throw new Exception($"Invalid Asset Company Name '{getCompany}' at Excel Row {currentRow}");
                            }
                            var amount = decimal.TryParse(worksheet.Cells[row, 31].Value?.ToString(), out decimal parsedAmount) ? parsedAmount : 0;
                            int? assetParentId = int.TryParse(worksheet.Cells[row, 10].Value?.ToString(), out int parsedAssetParentId)? parsedAssetParentId : (int?)null; 

                            var assetDto = new AssetMasterDto
                            {
                                //Asset
                                CompanyName=getCompanyId.ToString(),
                                UnitName=unitDetails.ShortName,
                                UnitId=request.ImportDto.UnitId,
                                CompanyId=request.ImportDto.CompanyId,
                                AssetGroupId = assetGroupId.Value ,     
                                AssetDescription ="",
                                MachineCode ="",                           
                                AssetCategoryId = assetCategoryId.Value ,
                                AssetSubCategoryId = assetSubCategoryId.Value,                                                                
                                AssetName = worksheet.Cells[row, 6].Value?.ToString(),                                
                                Quantity = int.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out int quantity) ? quantity : throw new Exception("Invalid Quantity"),
                                UOMId = assetUOMId.Value,
                                Active =bool.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out bool isActive) ? isActive : false,
                                AssetParentId = assetParentId,
                                WorkingStatus=1,                                
                                AssetType =(assetParentId != null && assetParentId > 0) ? 18 : 17 ,
                                 
                                AssetLocation = new AssetLocationCombineDto  
                                { 
                                    UnitId = assetUnitId ?? 0, 
                                    LocationId = assetLocationId ?? 1 , 
                                    SubLocationId = assetSubLocationId ?? 2 ,
                                    DepartmentId = assetDeptId ?? 0,
                                    CustodianId = int.TryParse(worksheet.Cells[row, 15].Value?.ToString(), out int custodianId) ? custodianId : 0
                                },  
                                AssetPurchaseDetails = new List<AssetPurchaseCombineDto>  
                                { 
                                    new AssetPurchaseCombineDto  
                                    {
                                        VendorCode = string.IsNullOrWhiteSpace(worksheet.Cells[row, 16].Value?.ToString()) ? "" : worksheet.Cells[row, 16].Value?.ToString()?.Trim(),
                                        VendorName =string.IsNullOrWhiteSpace(worksheet.Cells[row, 17].Value?.ToString()) ? "" : worksheet.Cells[row, 17].Value?.ToString()?.Trim(),
                                        PoNo = int.TryParse(worksheet.Cells[row, 18].Value?.ToString(), out int poNo) ? poNo : 0,
                                        PoDate = DateTimeOffset.TryParse(worksheet.Cells[row, 19].Value?.ToString(), out DateTimeOffset poDate) ? (DateTimeOffset?)poDate : null, 
                                        CapitalizationDate =DateTimeOffset.TryParse(worksheet.Cells[row, 20].Value?.ToString(), out DateTimeOffset capitalizationDate) ? (DateTimeOffset?)capitalizationDate : null,
                                        BillDate =DateTimeOffset.TryParse(worksheet.Cells[row, 21].Value?.ToString(), out DateTimeOffset billDate) ? (DateTimeOffset?)billDate :null,                                        
                                        BillNo = string.IsNullOrWhiteSpace(worksheet.Cells[row, 22].Value?.ToString()) ? "" : worksheet.Cells[row, 22].Value?.ToString()?.Trim(),
                                        PurchaseValue =decimal.TryParse(worksheet.Cells[row, 23].Value?.ToString(), out decimal price) ? price : 0,
                                        GrnNo =int.TryParse(worksheet.Cells[row, 24].Value?.ToString(), out int grnNo) ? grnNo : 0,
                                        GrnDate =DateTimeOffset.TryParse(worksheet.Cells[row, 25].Value?.ToString(), out DateTimeOffset grnDate) ? (DateTimeOffset?) grnDate : null,
                                        ItemCode=string.IsNullOrWhiteSpace(worksheet.Cells[row, 26].Value?.ToString()) ? "" : worksheet.Cells[row, 26].Value?.ToString()?.Trim(),
                                        ItemName=string.IsNullOrWhiteSpace(worksheet.Cells[row, 27].Value?.ToString()) ? "" : worksheet.Cells[row, 27].Value?.ToString()?.Trim(),
                                        OldUnitId=unitDetails.OldUnitId,
                                        PjDocId=string.IsNullOrWhiteSpace(worksheet.Cells[row, 28].Value?.ToString()) ? "" : worksheet.Cells[row, 28].Value?.ToString()?.Trim(),
                                        PjDocNo=int.TryParse(worksheet.Cells[row, 29].Value?.ToString(), out int pjDocNo) ? pjDocNo : 0,
                                        PjYear=string.IsNullOrWhiteSpace(worksheet.Cells[row, 30].Value?.ToString()) ? "" : worksheet.Cells[row, 30].Value?.ToString()?.Trim(),
                                        QcCompleted='Y',
                                        AssetSourceId=2,
                                        AcceptedQty=int.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out int acceptedQty) ? acceptedQty : throw new Exception("Invalid Quantity"),
                                        BudgetType="CAPIT",
                                    }
                                },                                  
                                AssetAssetAdditionalCost = amount > 0  
                                 ? new List<AssetAdditionalCostCombineDto>  
                                {                                    
                                        new AssetAdditionalCostCombineDto  
                                        {
                                            Amount = amount,
                                            JournalNo = string.IsNullOrWhiteSpace(worksheet.Cells[row, 32].Value?.ToString()) ? "" : worksheet.Cells[row, 32].Value?.ToString()?.Trim(),
                                            CostType=57 ,
                                            AssetSourceId=2                              
                                        }
                                    }
                                 : null // ✅ Set to null if amount <= 0
                                };                            
                            assetsDto.Add(assetDto);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error at Excel Row {currentRow}: {ex.Message}");
                        }
                    }
                }
            }
            //var assets = _mapper.Map<List<AssetMasterGenerals>>(assetsDto);            
            return await _assetRepository.ImportAssetsAsync(assetsDto,cancellationToken);
                
        }
    }
}