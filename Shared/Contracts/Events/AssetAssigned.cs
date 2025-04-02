using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public class AssetAssigned
    {
        public Guid CorrelationId { get; set; }
        public int AssetId { get; set; }
        public int UserId { get; set; }
    }
}