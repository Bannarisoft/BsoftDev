/* using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.Interfaces.IExcelImport;
using Core.Domain.Entities;
using MediatR;
using OfficeOpenXml;

namespace Core.Application.ExcelImport
{
    public class ImportAssetCommandHandlerold : IRequestHandler<ImportAssetCommand, bool>
    {
        private readonly IExcelImportCommandRepository _assetRepository;
        private readonly IExcelImportQueryRepository _assetQueryRepository;
        private readonly IMapper _mapper;


        public ImportAssetCommandHandlerold(IExcelImportCommandRepository assetRepository,IExcelImportQueryRepository assetQueryRepository ,IMapper mapper)
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
                       
                            //AssetLocation
                            string assetLocation = worksheet.Cells[row, 13].Value?.ToString() ?? string.Empty;                             
                            int? assetLocationId = await _assetRepository.GetAssetLocationIdByNameAsync(assetLocation);
                          
                            //AssetSubLocation
                            string assetSubLocation = worksheet.Cells[row, 14].Value?.ToString() ?? string.Empty;                             
                            int? assetSubLocationId = await _assetRepository.GetAssetSubLocationIdByNameAsync(assetSubLocation);
                         
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
                            //AssetParentId
                            int? assetParentId;
                            string assetParent = worksheet.Cells[row, 10].Value?.ToString() ?? string.Empty;   
                            if (assetParent == string.Empty)
                            {
                                assetParentId = (int?)null;
                            }                          
                            else{
                                assetParentId = await _assetRepository.GetAssetIdByNameAsync(assetParent);
                            }
                    
                            var assetSpecifications = new List<AssetSpecificationCombineDto>();
                            string? make = string.IsNullOrWhiteSpace(worksheet.Cells[row, 16].Value?.ToString()) ? null : worksheet.Cells[row, 16].Value?.ToString()?.Trim();
                            int? makeId = make == null ? null : 7; 
                           
                            string? modalNumber = string.IsNullOrWhiteSpace(worksheet.Cells[row, 17].Value?.ToString()) ? null : worksheet.Cells[row, 17].Value?.ToString()?.Trim();
                            int? modalNumberId = modalNumber == null ? null : 9; 
                           
                            string? serialNumber = string.IsNullOrWhiteSpace(worksheet.Cells[row, 18].Value?.ToString()) ? null : worksheet.Cells[row, 18].Value?.ToString()?.Trim();
                            int? serialNumberId = serialNumber == null ? null : 8; 
                           
                            string? capacity = string.IsNullOrWhiteSpace(worksheet.Cells[row, 19].Value?.ToString()) ? null : worksheet.Cells[row, 19].Value?.ToString()?.Trim();
                            int? capacityId = capacity == null ? null : 11; 
                            
                            string? uom = string.IsNullOrWhiteSpace(worksheet.Cells[row, 20].Value?.ToString()) ? null : worksheet.Cells[row, 20].Value?.ToString()?.Trim();
                            int? uomId = uom == null ? null : 4; 

                            string? power = string.IsNullOrWhiteSpace(worksheet.Cells[row, 21].Value?.ToString()) ? null : worksheet.Cells[row, 21].Value?.ToString()?.Trim();
                            int? powerId = uom == null ? null : 12; 
                             
                            string? rpm = string.IsNullOrWhiteSpace(worksheet.Cells[row, 22].Value?.ToString()) ? null : worksheet.Cells[row, 22].Value?.ToString()?.Trim();
                            int? rpmId = rpm == null ? null : 13; 

                            string? doi = string.IsNullOrWhiteSpace(worksheet.Cells[row, 23].Value?.ToString()) ? null : worksheet.Cells[row, 23].Value?.ToString()?.Trim();
                            int? doiId = doi == null ? null : 14; 
                            
                            string? prodSpec = string.IsNullOrWhiteSpace(worksheet.Cells[row, 24].Value?.ToString()) ? null : worksheet.Cells[row, 24].Value?.ToString()?.Trim();
                            int? prodSpecId = prodSpec == null ? null : 15; 

                            string? size = string.IsNullOrWhiteSpace(worksheet.Cells[row, 25].Value?.ToString()) ? null : worksheet.Cells[row, 25].Value?.ToString()?.Trim();
                            int? sizeId = size == null ? null : 16; 

                            string? weight = string.IsNullOrWhiteSpace(worksheet.Cells[row, 26].Value?.ToString()) ? null : worksheet.Cells[row, 26].Value?.ToString()?.Trim();
                            int? weightId = weight == null ? null : 17; 

                            string? color = string.IsNullOrWhiteSpace(worksheet.Cells[row, 27].Value?.ToString()) ? null : worksheet.Cells[row, 27].Value?.ToString()?.Trim();
                            int? colorId = color == null ? null : 18; 
                        
                            string? engineNumber = string.IsNullOrWhiteSpace(worksheet.Cells[row, 28].Value?.ToString()) ? null : worksheet.Cells[row, 28].Value?.ToString()?.Trim();
                            int? engineNumberId = engineNumber == null ? null : 19; 

                            string? chasisNumber = string.IsNullOrWhiteSpace(worksheet.Cells[row, 29].Value?.ToString()) ? null : worksheet.Cells[row, 29].Value?.ToString()?.Trim();
                            int? chasisNumberId = chasisNumber == null ? null : 20;
                            
                            string? vehicleNumber = string.IsNullOrWhiteSpace(worksheet.Cells[row, 30].Value?.ToString()) ? null : worksheet.Cells[row, 30].Value?.ToString()?.Trim();
                            int? vehicleNumberId = vehicleNumber == null ? null : 21;

                            string? TypeOfBuilding = string.IsNullOrWhiteSpace(worksheet.Cells[row, 31].Value?.ToString()) ? null : worksheet.Cells[row, 31].Value?.ToString()?.Trim();
                            int? TypeOfBuildingId = TypeOfBuilding == null ? null : 22;
                            
                            string? NoOfRooms = string.IsNullOrWhiteSpace(worksheet.Cells[row, 32].Value?.ToString()) ? null : worksheet.Cells[row, 32].Value?.ToString()?.Trim();
                            int? NoOfRoomsId = NoOfRooms == null ? null : 23;

                            string? ownership = string.IsNullOrWhiteSpace(worksheet.Cells[row, 33].Value?.ToString()) ? null : worksheet.Cells[row, 33].Value?.ToString()?.Trim();
                            int? ownershipId = ownership == null ? null : 24;

                            string? yearOfConstruction = string.IsNullOrWhiteSpace(worksheet.Cells[row, 34].Value?.ToString()) ? null : worksheet.Cells[row, 34].Value?.ToString()?.Trim();
                            int? yearOfConstructionId = yearOfConstruction == null ? null : 25;

                            string? NoOfFloors = string.IsNullOrWhiteSpace(worksheet.Cells[row, 35].Value?.ToString()) ? null : worksheet.Cells[row, 35].Value?.ToString()?.Trim();
                            int? NoOfFloorsId = NoOfFloors == null ? null : 26;


                            // Parse and validate Make                            
                            if (modalNumber != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = modalNumberId,
                                    SpecificationValue = modalNumber                                    
                                });
                            }
                            if (make != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = makeId,
                                    SpecificationValue = make                                    
                                });
                            }                            
                            if (serialNumber != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = serialNumberId,
                                    SpecificationValue = serialNumber                                    
                                });
                            }
                            if (capacity != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = capacityId,
                                    SpecificationValue = capacity                                    
                                });
                            }
                            if (uom != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = uomId,
                                    SpecificationValue = uom                                    
                                });
                            }
                            if (power != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = powerId,
                                    SpecificationValue = power                                    
                                });
                            }
                            if (rpm != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = rpmId,
                                    SpecificationValue = rpm                                    
                                });
                            }
                            if (doi != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = doiId,
                                    SpecificationValue = doi                                    
                                });
                            }
                            if (prodSpec != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = prodSpecId,
                                    SpecificationValue = prodSpec                                    
                                });
                            }
                            if (size != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = sizeId,
                                    SpecificationValue = size                                    
                                });
                            }
                            if (weight != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = weightId,
                                    SpecificationValue = weight                                    
                                });
                            }
                            if (color != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = colorId,
                                    SpecificationValue = color                                    
                                });
                            }
                            if (engineNumber != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = engineNumberId,
                                    SpecificationValue = engineNumber                                    
                                });
                            }
                            if (chasisNumber != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = chasisNumberId,
                                    SpecificationValue = chasisNumber                                    
                                });
                            }
                            if (vehicleNumber != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = vehicleNumberId,
                                    SpecificationValue = vehicleNumber                                    
                                });
                            }
                            if (TypeOfBuilding != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = TypeOfBuildingId,
                                    SpecificationValue = TypeOfBuilding                                    
                                });
                            }
                            if (NoOfRooms != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = NoOfRoomsId,
                                    SpecificationValue = NoOfRooms                                    
                                });
                            }
                            if (ownership != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = ownershipId,
                                    SpecificationValue = ownership                                    
                                });
                            }
                            if (yearOfConstruction != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = yearOfConstructionId,
                                    SpecificationValue = yearOfConstruction                                    
                                });
                            }
                            if (NoOfFloors != null)
                            {
                                assetSpecifications.Add(new AssetSpecificationCombineDto  
                                {               
                                    SpecificationId = NoOfFloorsId,
                                    SpecificationValue = NoOfFloors                                    
                                });
                            }
                            var amount = decimal.TryParse(worksheet.Cells[row, 47].Value?.ToString(), out decimal parsedAmount) ? parsedAmount : 0;
                            //int? assetParentId = int.TryParse(worksheet.Cells[row, 10].Value?.ToString(), out int parsedAssetParentId)? parsedAssetParentId : (int?)null; 
                            string policyAmount = string.IsNullOrWhiteSpace(worksheet.Cells[row, 51].Value?.ToString()) ? "0" : worksheet.Cells[row, 51].Value?.ToString()?.Trim();
                            string policyNo = string.IsNullOrWhiteSpace(worksheet.Cells[row, 48].Value?.ToString()) ? "" : worksheet.Cells[row, 48].Value?.ToString()?.Trim();
                            bool isPolicyAmountValid = decimal.TryParse(policyAmount, out decimal policyAmt) && policyAmt > 0;
                            bool isValid = isPolicyAmountValid && policyNo.Length > 0;


                            // Purchase
                            string vendorCode = worksheet.Cells[row, 36].Value?.ToString()?.Trim();
                            string vendorName = worksheet.Cells[row, 37].Value?.ToString()?.Trim();
                            int poNo = int.TryParse(worksheet.Cells[row, 38].Value?.ToString(), out int parsedPoNo) ? parsedPoNo : 0;
                            DateTimeOffset? poDate = DateTimeOffset.TryParse(worksheet.Cells[row, 39].Value?.ToString(), out DateTimeOffset parsedPoDate) ? (DateTimeOffset?)parsedPoDate : null;
                            string pjYear = worksheet.Cells[row, 40].Value?.ToString()?.Trim();
                            DateTimeOffset? billDate = DateTimeOffset.TryParse(worksheet.Cells[row, 41].Value?.ToString(), out DateTimeOffset parsedBillDate) ? (DateTimeOffset?)parsedBillDate : null;
                            string billNo = worksheet.Cells[row, 42].Value?.ToString()?.Trim();
                            
                            decimal purchaseValue = decimal.TryParse(worksheet.Cells[row, 43].Value?.ToString(), out decimal price) ? price : 0;
                            DateTimeOffset? grnDate = DateTimeOffset.TryParse(worksheet.Cells[row, 44].Value?.ToString(), out DateTimeOffset parsedGrnDate) ? (DateTimeOffset?)parsedGrnDate : null;

                            // Check if any of the values are filled in Excel
                            bool isAssetPurchaseValid = 
                                !string.IsNullOrWhiteSpace(vendorCode) || 
                                !string.IsNullOrWhiteSpace(vendorName) || 
                                poNo > 0 ||
                                poDate.HasValue || 
                                !string.IsNullOrWhiteSpace(pjYear) || 
                                billDate.HasValue ||
                                !string.IsNullOrWhiteSpace(billNo) || 
                                capitalizationDate.HasValue || 
                                purchaseValue > 0 || 
                                grnDate.HasValue;

                            //Location
                            int unitId = assetUnitId ?? 0;
                            int locationId = assetLocationId ?? 1;
                            int subLocationId = assetSubLocationId ?? 2;
                            int departmentId = assetDeptId ?? 0;
                            int custodianId = int.TryParse(worksheet.Cells[row, 15].Value?.ToString(), out int parsedCustodianId) ? parsedCustodianId : 0;

                            // Check if any of the values are filled in Excel
                            bool isAssetLocationValid = unitId > 0 || locationId > 0 || subLocationId > 0 || departmentId > 0 || custodianId > 0;

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
                                 
                                AssetLocation = isAssetLocationValid
                                ? new AssetLocationCombineDto  
                                { 
                                    UnitId = unitId,
                                    LocationId = locationId,
                                    SubLocationId = subLocationId,
                                    DepartmentId = departmentId,
                                    CustodianId = custodianId
                                } : null,                                
                                AssetPurchaseDetails = isAssetPurchaseValid
                                ? new List<AssetPurchaseCombineDto>
                                {
                                    new AssetPurchaseCombineDto  
                                    {
                                        VendorCode = string.IsNullOrWhiteSpace(vendorCode) ? "" : vendorCode,
                                        VendorName = string.IsNullOrWhiteSpace(vendorName) ? "" : vendorName,
                                        PoNo = poNo,
                                        PoDate = poDate,
                                        PjYear = string.IsNullOrWhiteSpace(pjYear) ? "" : pjYear,
                                        BillDate = billDate,
                                        BillNo = string.IsNullOrWhiteSpace(billNo) ? "" : billNo,
                                        CapitalizationDate = capitalizationDate,
                                        PurchaseValue = purchaseValue,
                                        GrnNo = 0,
                                        GrnDate = grnDate,
                                        ItemCode = "",
                                        ItemName = "",
                                        OldUnitId = unitDetails.OldUnitId,
                                        PjDocId = "",
                                        PjDocNo = 0,
                                        QcCompleted = 'Y',
                                        AssetSourceId = 2,
                                        AcceptedQty = 0,
                                        BudgetType = "CAPIT"
                                    }
                                }       : null ,                                  
                                AssetAdditionalCost = amount > 0  
                                 ? new List<AssetAdditionalCostCombineDto>  
                                {                                    
                                        new AssetAdditionalCostCombineDto  
                                        {
                                            Amount = amount,
                                            JournalNo = "",
                                            CostType=57 ,
                                            AssetSourceId=2                              
                                        }
                                    }
                                 : null ,
                                AssetSpecification = assetSpecifications.Count > 0 ? assetSpecifications : null  ,
                                AssetInsurance =isValid
                                ?  new List<AssetInsuranceCombineDto>  
                                { 
                                    new AssetInsuranceCombineDto  
                                    {
                                        PolicyNo=string.IsNullOrWhiteSpace(worksheet.Cells[row, 48].Value?.ToString()) ? "" : worksheet.Cells[row, 48].Value?.ToString()?.Trim(),
                                        StartDate=DateTimeOffset.TryParse(worksheet.Cells[row, 49].Value?.ToString(), out DateTimeOffset startDate) ? (DateTimeOffset?)startDate : null,
                                        EndDate=DateTimeOffset.TryParse(worksheet.Cells[row, 50].Value?.ToString(), out DateTimeOffset endDate) ? (DateTimeOffset?)endDate : null,
                                        PolicyAmount=string.IsNullOrWhiteSpace(worksheet.Cells[row, 51].Value?.ToString()) ? "" : worksheet.Cells[row, 51].Value?.ToString()?.Trim(),
                                        VendorCode=string.IsNullOrWhiteSpace(worksheet.Cells[row, 52].Value?.ToString()) ? "" : worksheet.Cells[row, 52].Value?.ToString()?.Trim()
                                    }
                                } : null 
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
} */