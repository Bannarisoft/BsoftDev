using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public interface IAssetAssigned
    {
        Guid AssetId { get; }
        Guid UserId { get; }
    }
}