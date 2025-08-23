using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Dtos.Purchase
{
    public class UpdateApprovedQtyDto
    {
        public int IndentDetailId { get; set; }
        public decimal ApprovedQuantity { get; set; }
    }
}