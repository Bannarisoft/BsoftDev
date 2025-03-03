using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public interface IAssetReleased
    {
        Guid AssetId { get; }
        Guid UserId { get; }
    }
}