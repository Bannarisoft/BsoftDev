using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.Interfaces;
using OfficeOpenXml;

namespace Core.Application.ExcelImport
{
    public class AssetPurchaseHandler
    {
        private readonly ExcelWorksheet _worksheet;
        private readonly int _row;
        private readonly IIPAddressService _ipAddressService;

        public AssetPurchaseHandler(ExcelWorksheet worksheet, int row, IIPAddressService ipAddressService)
        {
            _worksheet = worksheet;
            _row = row;
            _ipAddressService = ipAddressService;
        }

        public List<AssetPurchaseCombineDto>? ProcessAssetPurchase()
        {
            var oldUnitId = _ipAddressService.GetOldUnitId();
            string vendorCode = _worksheet.Cells[_row, 36].Value?.ToString()?.Trim();
            string vendorName = _worksheet.Cells[_row, 37].Value?.ToString()?.Trim();
            int poNo = int.TryParse(_worksheet.Cells[_row, 38].Value?.ToString(), out int parsedPoNo) ? parsedPoNo : 0;
            DateTimeOffset? poDate = DateTimeOffset.TryParse(_worksheet.Cells[_row, 39].Value?.ToString(), out DateTimeOffset parsedPoDate) ? (DateTimeOffset?)parsedPoDate : null;
            string pjYear = _worksheet.Cells[_row, 40].Value?.ToString()?.Trim();
            DateTimeOffset? billDate = DateTimeOffset.TryParse(_worksheet.Cells[_row, 41].Value?.ToString(), out DateTimeOffset parsedBillDate) ? (DateTimeOffset?)parsedBillDate : null;
            string billNo = _worksheet.Cells[_row, 42].Value?.ToString()?.Trim();
            decimal purchaseValue = decimal.TryParse(_worksheet.Cells[_row, 43].Value?.ToString(), out decimal price) ? price : 0;

            // Check if any value exists in Excel
            bool isValid = !string.IsNullOrWhiteSpace(vendorCode) || !string.IsNullOrWhiteSpace(vendorName) || poNo > 0 ||
                           poDate.HasValue || !string.IsNullOrWhiteSpace(pjYear) || billDate.HasValue ||
                           !string.IsNullOrWhiteSpace(billNo) || purchaseValue > 0;

            if (!isValid) return null;

            return new List<AssetPurchaseCombineDto>
            {
                new AssetPurchaseCombineDto
                {
                    VendorCode = vendorCode ?? string.Empty,
                    VendorName = vendorName ?? string.Empty,
                    PoNo = poNo,
                    PoDate = poDate,
                    PjYear = pjYear ?? string.Empty,
                    BillDate = billDate,
                    BillNo = billNo ?? string.Empty,
                    PurchaseValue = purchaseValue,
                    GrnNo = 0,
                    ItemCode = string.Empty,
                    ItemName = string.Empty,
                    QcCompleted = 'Y',
                    AssetSourceId = 2,
                    AcceptedQty = 0,
                    BudgetType = "CAPIT",
                    OldUnitId=oldUnitId,PjDocId=string.Empty
                }
            };
        }
    }
}
